using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Server.Tools.MessageMapping;

public class AssemblyScaner
{
    readonly HashSet<Assembly> _assemblies = new HashSet<Assembly>();
    readonly IDictionary<Type, Type[]> _dispatcher = new Dictionary<Type, Type[]>();

    AssemblyScaner()
    {
    }

    public Type[] ClassInterfaces { get; set; }
    public Type[] GenericDefs { get; set; }

    public static IDictionary<Type, Type[]> GetCommandHandlerMap(Action<AssemblyScaner> config)
    {
        var scanner = new AssemblyScaner();
        config(scanner);
        scanner.Build();
        return scanner._dispatcher;
    }

    public static IEnumerable<Type> GetCommandHandlers(IEnumerable<Type> genericDefs)
    {
        var scanner = new AssemblyScaner();
        return scanner.GetHandlers(genericDefs);
    }

    public static IEnumerable<Type> GetCommandTypes(IEnumerable<Type> classInterfaces)
    {
        var scanner = new AssemblyScaner();
        return scanner.ParseCommandTypes(classInterfaces);

    }

    IEnumerable<Type> ParseCommandTypes(IEnumerable<Type> classInterfaces)
    {
        FillAssemblies();
        var types = _assemblies
            .SelectMany(a => a.GetExportedTypes())
            .ToList();

        var messageTypes = types
            .Where(t => classInterfaces.Any(n => t.InheritsOrImplements(new[] { n })))
            .ToArray();
        return messageTypes;
    }

    IEnumerable<Type> GetHandlers(IEnumerable<Type> genericDefs)
    {
        FillAssemblies();
        var types = _assemblies
            .SelectMany(a => a.GetExportedTypes())
            .ToList();

        var consumerTypes = types
            .Where(t => genericDefs.Any(n => t.InheritsOrImplements(new[] { n })))
            .Where(t => !t.IsGenericType)
            .ToArray();
        return consumerTypes;
    }

    void Build()
    {
        var mapping = MessageActivationInfoBuilder();
        var messageActivationInfos = BuildActivationMap(mapping);
        foreach (var message in messageActivationInfos)
        {
            if (message.AllConsumers.Length > 0)
            {
                _dispatcher.Add(message.MessageType, message.AllConsumers);
            }
        }
    }

    MessageMapping[] MessageActivationInfoBuilder()
    {
        FillAssemblies();

        var types = _assemblies
            .SelectMany(a => a.GetExportedTypes())
            .ToList();

        var messageTypes = types
            .Where(t => t.InheritsOrImplements(ClassInterfaces) && ClassInterfaces.Any(n => n != t))
            .ToArray();

        var consumerTypes = types
            .Where(t => t.InheritsOrImplements(GenericDefs))
            .Where(t => !t.IsGenericType)
            .ToArray();

        var consumingDirectly = consumerTypes
            .SelectMany(consumerType =>
                TypeExtension.ListMessagesConsumedByInterfaces(consumerType, GenericDefs)
                    .Select(messageType => new MessageMapping(consumerType, messageType)))
            .ToArray();

        var allMessages = new HashSet<Type>(consumingDirectly.Select(m => m.Message));
        foreach (var messageType in messageTypes)
        {
            if (!allMessages.Contains(messageType))
            {
                throw new ArgumentException(string.Format("Handler for message{0} was not found.",
                    messageType));
            }
        }
        return consumingDirectly;
    }

    void FillAssemblies()
    {
        if (!_assemblies.Any())
        {
            IEnumerable<Assembly> userAssemblies =
                AppDomain.CurrentDomain.GetAssemblies().Where(AssemblyScanEvil.IsProbablyUserAssembly);
            foreach (Assembly userAssembly in userAssemblies)
            {
                _assemblies.Add(userAssembly);
            }
        }
    }

    public MessageActivationInfo[] BuildActivationMap(MessageMapping[] mappings
        /*, Func<MessageMapping, bool> filter*/)
    {
        return mappings
            //             .Where(filter)
            .GroupBy(x => x.Message)
            .Select(x => new MessageActivationInfo
            {
                MessageType = x.Key,
                AllConsumers = x
                    .Select(m => m.Consumer)
                    .Distinct()
                    .ToArray(),
            })
            .ToArray();
    }

    public void AddAssembly(Assembly assembly)
    {
        _assemblies.Add(assembly);
    }
}