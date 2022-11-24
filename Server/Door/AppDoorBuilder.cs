using BTDB.IOC;

namespace Server.Door;

public class AppDoorBuilder
{
    public static void Build(ContainerBuilder builder)
    {
        builder.RegisterType<DoorMqttClient>().As<DoorMqttClient>().SingleInstance();
    }
}