
namespace Server;

public class ServerOptions
{
    public int HttpServerConnectionPort { get; init; }
    public string DatabaseCollectionPath { get; init; }
    public string MqttClientAddress { get; init; }
    public string HttpRooDirectory { get; init; }

    public string ImagesRootDirectory { get; init; }
}