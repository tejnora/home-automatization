using BTDB.IOC;

namespace HttpServer.Door
{
    public class AppDoorBuilder
    {
        public static void Build(ContainerBuilder builder)
        {
            builder.RegisterType<DoorMqttClient>().As<DoorMqttClient>().SingleInstance();
            //AppBuilder.RegisterCommandQueryHandlers(builder, "HttpServer.Door");
        }
    }
}