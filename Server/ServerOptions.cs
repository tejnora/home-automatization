
namespace Server;

public class ServerOptions
{
    public int HttpServerConnectionPort { get; init; }
    public string DatabaseCollectionPath { get; init; }
    public string MqttClientAddress { get; init; }
}