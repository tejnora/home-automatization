using System;

namespace Server.HttpServer
{
    class DebugHttpFileLoader
        : IHttpFileLoader
    {
        public bool TryGetWebFileContent(string[] path, out byte[] content)
        {
            throw new NotImplementedException();
        }
    }
}
