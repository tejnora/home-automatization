using System.Threading.Tasks;

namespace Server.HttpServer;

public interface IHttpFileLoader
{
    Task<byte[]> TryGetWebFileContent(string[] path);
}