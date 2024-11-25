using Server.Tools;
using BTDB.IOC;
using BTDB.KVDBLayer;
using System;
using System.Configuration;
using Server.Storage;
using Serilog;
using IContainer = BTDB.IOC.IContainer;
using Server.HttpServer;
using Server.Core;
using System.IO;
using Server.Tools.MessageMapping;
using Server.Authentication;
using WebCommandsList = Server.HttpServer.WebCommandsList;
using Server.Door;
using Server.Mqtt;
using Server.Core.Mqtt;
using System.Text;
using Server.ImageGallery;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            var provider = CodePagesEncodingProvider.Instance;
            Encoding.RegisterProvider(provider);

            LocConfiguration();
            NameAndVersionReporter.Print();
            UnhandledExceptionHandler.AttachHandlers();
            var options = InitServerOptions();
            if (!Directory.Exists(options.DatabaseCollectionPath))
            {
                Directory.CreateDirectory(options.DatabaseCollectionPath);
            }
            var builder = new ContainerBuilder();
            builder.RegisterType<HttpServer.HttpServer>().As<IHttpServer>().SingleInstance();
            builder.RegisterInstance(options).As<ServerOptions>();
            builder.RegisterInstance(new OnDiskFileCollection(options.DatabaseCollectionPath)).As<IFileCollection>();
            builder.RegisterType<Clock>().As<IClock>().SingleInstance();
            builder.RegisterType<WebCommandsList>().AsSelf().SingleInstance();
            builder.RegisterType<DataStorage>().As<IDataStorage>().SingleInstance();
            builder.RegisterType<SessionMiddlewareApi>().As<IRestApi>().SingleInstance();
            builder.RegisterType<SessionManager>().As<ISessionManager>().SingleInstance();
            builder.RegisterType<DoorMqttClient>().AsSelf().SingleInstance();
            builder.RegisterType<MqttClientWrapper>().As<IMqttClient>().SingleInstance();
            builder.RegisterType<ImagePreviewCache>().As<IImagePreviewCache>().SingleInstance();
#if DEBUG
            // builder.RegisterType<DebugHttpFileLoader>().As<IHttpFileLoader>().SingleInstance();
            builder.RegisterType<DiskHttpFileLoader>().As<IHttpFileLoader>().SingleInstance();
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
#if DEBUG
            var dbCollection = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath),"DiskFileCollection");
            return new ServerOptions
            {
                DatabaseCollectionPath = "DiskFileCollection",
                MqttClientAddress= "192.168.88.250:1883",
                HttpServerConnectionPort = 80,
                HttpRooDirectory = @"c:\_Data\Repos\home-automatization\frontend\build\",
                ImagesRootDirectory= @"c:\_Data\"
            };
#else
            try
            {
                var dbCollection = Path.Combine(Path.GetDirectoryName(Environment.ProcessPath), ConfigurationManager.AppSettings["DiskFileCollection"]);
                return new ServerOptions
                {
                    DatabaseCollectionPath = dbCollection,
                    MqttClientAddress = ConfigurationManager.AppSettings["MqttClientAddress"],
                    HttpServerConnectionPort = int.Parse(ConfigurationManager.AppSettings["HttpServerConnectionPort"]),
                    HttpRooDirectory = ConfigurationManager.AppSettings["HttpRooDirectory"],
                    ImagesRootDirectory = ConfigurationManager.AppSettings["ImagesRootDirectory"]
                };
            }
            catch (Exception ex)
            {
                Log.Error($"Config option cannot be read. {ex}");
                Environment.Exit(1);
            }
#endif
            return null;
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
            }
        }
    }
}
