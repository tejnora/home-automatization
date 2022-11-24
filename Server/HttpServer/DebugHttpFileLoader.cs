using System;
using System.IO;
using System.Threading.Tasks;

namespace Server.HttpServer;

class DebugHttpFileLoader
    : IHttpFileLoader
{
    static readonly string RootPath = @"e:\Repozitories\home-automatization\www\";
    public async Task<byte[]> TryGetWebFileContent(string[] path)
    {
        var finalPath = Path.Combine(RootPath, Path.Combine(path));
        if (!File.Exists(finalPath))
        {
            return await Task.FromResult<byte[]>(null);
        }
        try
        {
            return  await File.ReadAllBytesAsync(finalPath);
        }
        catch
        {
            return await Task.FromResult<byte[]>(null);
        }
        
    }
}