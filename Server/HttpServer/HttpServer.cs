using System;
using System.IO;
using BTDB.IOC;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Server.HttpServer;

public class HttpServer : IHttpServer
{
    IWebHost _webServer;
    readonly int _connectionPort;
    readonly IContainer _container;

    public HttpServer(ServerOptions serverOptions, IContainer container)
    {
        _connectionPort = serverOptions.HttpServerConnectionPort;
        _container = container;
    }

    public void Dispose()
    {
        StopListening();
    }

    public void StartListening()
    {
#if (DEBUG && !DEBUG_RELEASE)
        var hostName = "localhost";
#else
            var hostName = System.Net.Dns.GetHostName();
#endif
        try
        {
            _webServer = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls($"http://{hostName}:{_connectionPort}/")
                .ConfigureServices(services =>
                {
                    services.AddSingleton(_container);
                    services.AddSingleton(this);
                    services.Configure<KestrelServerOptions>(options => options.Limits.MaxRequestBodySize = 1000000);
                })
                .UseStartup<HttpHandler>()
                .Build();
            _webServer.Start();
            Log.Information($"Listen on http://{hostName}:{_connectionPort}/");
        }
        catch (Exception ex)
        {
            Log.Error($"Http server can not be started. {ex}");
        }

    }

    public void StopListening()
    {
        if (_webServer == null) return;
        _webServer.StopAsync().Wait();
        _webServer.Dispose();
    }
}