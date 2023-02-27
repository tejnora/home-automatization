using Server.Tools;
using BTDB.IOC;
using BTDB.KVDBLayer;
using Server.Configuration;
using System;
using Server.Storage;
using Serilog;
using IContainer = BTDB.IOC.IContainer;
using Server.HttpServer;
using Server.Core;
using System.IO;
using Server.Tools.MessageMapping;
using Server.Authentication;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            LocConfiguration();
            NameAndVersionReporter.Print();
            UnhandledExceptionHandler.AttachHandlers();
            var options = InitServerOptions();
            var builder = new ContainerBuilder();
            builder.RegisterType<HttpServer.HttpServer>().As<IHttpServer>().SingleInstance();
            builder.RegisterInstance(options).As<ServerOptions>();
            builder.RegisterInstance(new OnDiskFileCollection(options.DatabaseCollectionPath)).As<IFileCollection>();
            builder.RegisterType<Clock>().As<IClock>().SingleInstance();
            builder.RegisterType<WebCommandsList>().AsSelf().SingleInstance();
            builder.RegisterType<DataStorage>().As<IDataStorage>().SingleInstance();
            builder.RegisterType<SessionMiddlewareApi>().As<IRestApi>().SingleInstance();
            builder.RegisterType<SessionManager>().As<ISessionManager>().SingleInstance();
#if DEBUG
            builder.RegisterType<DebugHttpFileLoader>().As<IHttpFileLoader>().SingleInstance();
#else
            builder.RegisterType<DiskHttpFileLoader>().As<IHttpFileLoader>().SingleInstance();
#endif 
            RegisterCommandHandlers(builder);
            var container = builder.Build();
            Run(container);
        }

        static void LocConfiguration()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Debug()
                .CreateLogger();
        }
        static void RegisterCommandHandlers(ContainerBuilder builder)
        {
            var commandHandlers = AssemblyScaner.GetCommandHandlers(new[]
            {
                typeof(Define.IConsumer<>),
                typeof(Define.IConsumer<,>),
                typeof(Define.IQuery<,>),
            });
            foreach (var handler in commandHandlers)
            {
                builder.RegisterType(handler).AsSelf().AsImplementedInterfaces().SingleInstance();
            }
        }

        static ServerOptions InitServerOptions()
        {
            return new ServerOptions
            {
                DatabaseCollectionPath = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath),DatabaseConfigSection.GetConfiguration().DiskFileCollection),
                HttpServerConnectionPort = 80,
                MqttClientAddress= "tcp://192.168.88.250:1883"
            };
        }

        static void Run(IContainer container)
        {
            var dataStorage = container.Resolve<IDataStorage>();
            var dateServer = container.Resolve<IHttpServer>();
            try
            {
                try
                {
                    dateServer.StartListening();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
                while (true)
                {
                    var value = Console.ReadLine();
                    if (value != "q") continue;
                    dateServer.StopListening();
                    break;
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.ToString());
                Console.ReadLine();
            }
            finally
            {
                ((IDisposable)dataStorage).Dispose();
                ((IDisposable)dateServer).Dispose();
            }

        }
    }
}
