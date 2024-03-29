﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BTDB.IOC;
using BTDB.KVDBLayer;
using BTDB.ODBLayer;
using Serilog;
using Server.Core;
using Server.Tools;
using Server.Tools.MessageMapping;

namespace Server.Storage;

public class DataStorage
    : IRestApi
    , IDataStorage
    , IDisposable
{

    readonly AsyncSemaphore _writeLock = new AsyncSemaphore(1);
    readonly IDictionary<Type, Type[]> _queryMapInfo;
    readonly IDictionary<Type, Type[]> _commandMapInfo;
    readonly Dictionary<Type, object> _queryObjectsCache = new Dictionary<Type, object>();
    readonly Dictionary<Type, object> _commandObjectsCache = new Dictionary<Type, object>();
    readonly object _objectsCacheLock = new object();
    readonly IFileCollection _fileCollection;
    readonly IContainer _container;
    public IObjectDB Database { get; }

    public DataStorage(IFileCollection fileCollection, IContainer container)
    {
        _fileCollection = fileCollection;
        var databaseKeyValueTree = new BTreeKeyValueDB(_fileCollection);
        Database = new ObjectDB();
        Database.Open(databaseKeyValueTree, true);

        _container = container;
        _queryMapInfo = AssemblyScaner.GetCommandHandlerMap(h =>
        {
            h.ClassInterfaces = new[] { typeof(Define.IRequest) };
            h.GenericDefs = new[] { typeof(Define.IQuery<,>) };
        });

        _commandMapInfo = AssemblyScaner.GetCommandHandlerMap(h =>
        {
            h.ClassInterfaces = new[] { typeof(Define.ICommand) };
            h.GenericDefs = new[] { typeof(Define.IConsumer<>), typeof(Define.IConsumer<,>) };
        });
        Command(new CommandContextBase(null), new InitCommand());
    }

    public void Dispose()
    {
        _writeLock.WaitAsync().ContinueWith(_ =>
        {
            Database?.Dispose();
            _fileCollection?.Dispose();
        }).Wait();
    }

    public Task<List<Define.IResponse>> Command(ICommandContext commandContext, Define.ICommand commnad)
    {
        if (!_commandMapInfo.TryGetValue(commnad.GetType(), out var consumerTypes))
        {
            throw new ArgumentException($"Handler for command {commnad.GetType()} does not exist.");
        }
        var source = new TaskCompletionSource<List<Define.IResponse>>();
        _writeLock.WaitAsync().ContinueWith(t =>
        {
            try
            {
                commandContext.Transaction = Database.StartTransaction();
                var results = new List<Define.IResponse>();
                foreach (var consumerType in consumerTypes)
                {
                    if (!_commandObjectsCache.TryGetValue(consumerType, out var instance))
                    {
                        instance = _container.Resolve(consumerType);
                        _commandObjectsCache.Add(consumerType, instance);
                    }
                    var consume = consumerType.GetMethod("Consume", new[] { commandContext.GetType(), commnad.GetType() });
                    if (consume == null)
                    {
                        Log.Error($"Consume method for command {commnad.GetType()} does not exist.");
                        continue;
                    }
                    var result = consume.Invoke(instance, new object[] { commandContext, commnad });
                    if (result != null) results.Add((Define.IResponse)result);
                }
                source.SetResult(results);
                commandContext.Transaction.Commit();
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }
            finally
            {
                commandContext.Transaction.Dispose();
                commandContext.Transaction = null;
                _writeLock.Release();
            }
        }, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
        return source.Task;
    }

    public Task<Define.IResponse> Query(IQueryContext queryContext, Define.IRequest query)
    {
        return Task.Factory.StartNew(() =>
        {
            if (!_queryMapInfo.TryGetValue(query.GetType(), out var consumerTypes))
            {
                throw new ArgumentException($"Handler for query {query.GetType()} does not exist.");
            }

            var consumerType = consumerTypes[0];
            object instance;
            MethodInfo consume;
            lock (_objectsCacheLock)
            {
                if (!_queryObjectsCache.TryGetValue(consumerType, out instance))
                {
                    instance = _container.Resolve(consumerType);
                    _queryObjectsCache.Add(consumerType, instance);
                }
                consume = consumerType.GetMethod("Consume", new[] { queryContext.GetType(), query.GetType() });
            }
            if (consume == null)
            {
                throw new ArgumentException($"Consume method for command {query.GetType().Name} does not exist.");
            }

            try
            {
                queryContext.Transaction = Database.StartReadOnlyTransaction();
                return (Define.IResponse)consume.Invoke(instance, new object[] { queryContext, query });
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return null;
            }
            finally
            {
                queryContext.Transaction.Dispose();
                queryContext.Transaction = null;
            }
        }, CancellationToken.None, TaskCreationOptions.None, TaskScheduler.Default);
    }
}