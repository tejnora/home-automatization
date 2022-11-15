using System;
using System.IO;

namespace Server.HttpServer
{
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
                // ignored
            }
            content = null;
            return false;
        }
    }
}