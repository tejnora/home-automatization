using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using BTDB.IOC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using Server.Core;
using Server.Tools;
using Server.Tools.JsonSerializer;

namespace Server.HttpServer;

public class HttpHandler
{
    readonly IHttpFileLoader _httpFileLoader;
    readonly IClock _clock;
    readonly IDictionary<string, Type> _getCommands = new Dictionary<string, Type>();
    readonly IDictionary<string, Type> _postCommands = new Dictionary<string, Type>();
    readonly IRestApi _restApi;
    static readonly JsonSerializerOptions JsonSerializerOptions = new() { Converters = { new JsonStringEnumConverter(), new UnixEpochDateTimeConverter(), new NullToEmptyStringConverter() } };
    static readonly JsonSerializerOptions JsonDeserializerReadOptions = new() { Converters = { new JsonStringEnumConverter(), new UnixEpochDateTimeConverter() } };


    public HttpHandler(IContainer container)
    {
        _clock = container.Resolve<IClock>();
        _httpFileLoader = container.Resolve<IHttpFileLoader>();
        _restApi = container.Resolve<IRestApi>();
        RegisterCommands(container.Resolve<WebCommandsList>());
    }

    void RegisterCommands(WebCommandsList commandLister)
    {
        foreach (var cmd in commandLister.ListGetCommands())
            _getCommands[cmd.Name] = cmd;
        foreach (var cmd in commandLister.ListPostCommands())
            _postCommands[cmd.Name] = cmd;
    }

    public void Configure(IApplicationBuilder app)
    {
        app.Run(async (httpContext) =>
        {
            if (httpContext.Request.Path.Value.Contains(".."))
            {
                await SetStatusCodeAndWriteMessageToResponse(httpContext.Response, 400, "Refuse handle file request containing '..'");
                return;
            }
            var requestPathFragments = GetPathFragments(httpContext.Request.Path.Value);
            if (requestPathFragments.Length == 0)
            {
                requestPathFragments = new[] { "index.html" };
            }

            if (requestPathFragments[0] == "api")
            {
                await ProcessHttpRequest(requestPathFragments, httpContext);
            }
            else
            {
                var fileContent = await _httpFileLoader.TryGetWebFileContent(requestPathFragments);
                if (fileContent == null)
                {
                    SetNotFoundStatus(httpContext);
                    return;
                }
                SetSuccessStatus(httpContext);
                SetResponseHeaders(httpContext.Response, requestPathFragments);
                await httpContext.Response.Body.WriteAsync(fileContent, 0, fileContent.Length);
            }
        });
    }

    async Task ProcessHttpRequest(IList<string> path, HttpContext httpContext)
    {
        switch (httpContext.Request.Method)
        {
            case "GET":
                await HandleApiGet(path[2], httpContext);
                break;
            case "POST":
                await HandleApiPost(path[2], httpContext);
                break;
            default:
                SetNotFoundStatus(httpContext);
                break;
        }
    }

    async Task HandleApiGet(string requestString, HttpContext httpContext)
    {
        if (!_getCommands.TryGetValue(requestString, out var commandType))
        {
            SetNotFoundStatus(httpContext);
            return;
        }
        try
        {
            var comm = (Define.IRequest)Activator.CreateInstance(commandType);
            var responseDto = await _restApi.Query(new QueryContextBase(httpContext), comm);
            await ReturnJson(responseDto, httpContext);
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
    }

    async Task HandleApiPost(string requestString, HttpContext context)
    {
        if (!_postCommands.TryGetValue(requestString, out var requestType))
        {
            SetNotFoundStatus(context);
            return;
        }
        try
        {
            var commandObject = await CreateCommand(requestType, context);
            var responseDto = await _restApi.Command(new CommandContextBase(context), commandObject);
            await ReturnJson(responseDto, context);
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
    }
    static async Task<Define.ICommand> CreateCommand(Type requestType, HttpContext httpContext)
    {
        string inputString;
        using (TextReader r = new StreamReader(httpContext.Request.Body, Encoding.UTF8))
        {
            inputString = await r.ReadToEndAsync();
        }
        return (Define.ICommand)(!string.IsNullOrEmpty(inputString) ? JsonSerializer.Deserialize(inputString, requestType, JsonDeserializerReadOptions) : Activator.CreateInstance(requestType));
    }


    static async Task ReturnJson(object data, HttpContext httpContext)
    {
        var jsonData = JsonSerializer.Serialize(data, JsonSerializerOptions);
        httpContext.Response.StatusCode = 200;
        httpContext.Response.ContentType = "application/json;charset=utf8";
        await httpContext.Response.Body.WriteAsync(new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(jsonData)));
    }

    void SetResponseHeaders(HttpResponse response, IEnumerable<string> requestPathFragments, bool allowCache = false)
    {
        var extension = Path.GetExtension(requestPathFragments.Last());
        response.ContentType = GetContentType(extension);
        if (extension is ".js" or ".css")
            response.Cookies.Append("Expires", _clock.UtcNow.ToString());
        if (allowCache)
        {
            response.Cookies.Append("Expires", _clock.UtcNow.AddYears(1).ToString());//TODO
            response.Headers["Cache-Control"] = "max-age=31536000; must-revalidate";
        }
        if (extension == ".html")
            response.Headers["X-Frame-Options"] = "SAMEORIGIN";
    }

    static string GetContentType(string extension)
    {
        switch (extension)
        {
            case ".avi": return "video/x-msvideo";
            case ".css": return "text/css";
            case ".doc": return "application/msword";
            case ".gif": return "image/gif";
            case ".htm":
            case ".html": return "text/html";
            case ".jpg":
            case ".jpeg": return "image/jpeg";
            case ".js": return "application/x-javascript";
            case ".mp3": return "audio/mpeg";
            case ".png": return "image/png";
            case ".pdf": return "application/pdf";
            case ".ppt": return "application/vnd.ms-powerpoint";
            case ".zip": return "application/zip";
            case ".txt": return "text/plain";
            case ".tif": return "image/tiff";
            case ".eot": return "application/vnd.ms-fontobject";
            case ".ttf": return "application/x-font-ttf";
            case ".otf": return "application/x-font-opentype";
            case ".woff": return "application/x-font-woff";
            case ".svg": return "image/svg+xml";
            default: return "application/octet-stream";
        }
    }

    static void SetSuccessStatus(HttpContext context)
    {
        SetStatusCode(context.Response, 200);
    }
    static void SetNotFoundStatus(HttpContext context)
    {
        SetStatusCode(context.Response, 404);
    }

    static async Task SetStatusCodeAndWriteMessageToResponse(HttpResponse response, int statusCode, string message = null)
    {
        SetStatusCode(response, statusCode, message);
        if (!string.IsNullOrEmpty(message))
        {
            await response.WriteAsync(message, Encoding.UTF8);
        }
    }

    static void SetStatusCode(HttpResponse response, int statusCode, string message = null)
    {
        if (statusCode >= 400)
            Log.Warning($"{statusCode} {message ?? string.Empty}");
        response.StatusCode = statusCode;
    }

    static string[] GetPathFragments(string path)
    {
        var res = path.Split(new[] { @"/", @"\" }, StringSplitOptions.RemoveEmptyEntries);
        return res;
    }
}