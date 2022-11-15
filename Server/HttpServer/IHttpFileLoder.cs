namespace Server.HttpServer;

public interface IHttpFileLoader
{
    bool TryGetWebFileContent(string[] path, out byte[] content);
}