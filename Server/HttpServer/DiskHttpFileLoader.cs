using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Server.HttpServer;

public class DiskHttpFileLoader
    : IHttpFileLoader
{
    readonly string _root;

    public DiskHttpFileLoader(ServerOptions serverOptions)
    {
        _root = serverOptions.HttpRooDirectory;
    }

    public Task<byte[]> TryGetWebFileContent(string[] path)
    {
        try
        {
            var filePath = Path.Combine(_root, Path.Combine(path));
            if (filePath.IndexOf(_root, StringComparison.Ordinal) != 0)
            {
                return null;
            }
            return File.ReadAllBytesAsync(filePath);
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
        return null;

    }
}