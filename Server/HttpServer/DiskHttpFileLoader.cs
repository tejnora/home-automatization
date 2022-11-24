using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Server.HttpServer;

public class DiskHttpFileLoader
    : IHttpFileLoader
{
    readonly string _root;

    public DiskHttpFileLoader()
    {
        _root = Path.Combine("/var/www/home.geodetka.eu/htdocs", @"Html");
    }
    public bool TryGetWebFileContent(string[] path, out byte[] content)
    {
        content = null;
        try
        {
            var filePath = Path.Combine(_root, Path.Combine(path));
            if (filePath.IndexOf("/var/www/home.geodetka.eu/htdocs", StringComparison.Ordinal) != 0)
            {
                return false;
            }
            content = File.ReadAllBytes(filePath);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
        content = null;
        return false;
    }

    public Task<byte[]> TryGetWebFileContent(string[] path)
    {
        throw new NotImplementedException();
    }
}