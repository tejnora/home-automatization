using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BTDB.IOC;
using BTDB.KVDBLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Serilog;
using Server.Core;
using Server.Tools;

namespace Server.HttpServer
{
    public class HttpHandler
    {
        readonly IContainer _container;
        readonly IHttpFileLoader _httpFileLoader;
        readonly IClock _clock;
        readonly IDictionary<string, Type> _getCommands = new Dictionary<string, Type>();
        readonly IDictionary<string, Type> _postCommands = new Dictionary<string, Type>();
        readonly IRestApi _restApi;


        public HttpHandler(IContainer container)
        {
            _container = container;
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
                    SetStatusCodeAndWriteMessageToResponse(httpContext.Response, 400, "Refuse handle file request containing '..'");
                    return;
                }
                var requestPathFragments = GetPathFragments(httpContext.Request.Path.Value);
                if (requestPathFragments.Length == 0)
                {
                    requestPathFragments = new string[] { "index.html" };
                }

                if (requestPathFragments[0] == "api")
                {
                    await ProcessHttpRequest(requestPathFragments,httpContext);
                }
                else
                {
                    if (_httpFileLoader.TryGetWebFileContent(requestPathFragments, out var fileContent))
                    {
                        await httpContext.Response.Body.WriteAsync(fileContent,0, fileContent.Length);
                        SetResponseHeaders(httpContext.Response, requestPathFragments);
                    }
                    else
                    {
                        SetNotFoundStatus(httpContext.Response);
                    }
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
                    {
                        if (path[2] == "MessagesWrapper")
                            await HandleArrayRequestMessages(httpContext);
                        else
                            await HandleApiPost(path[2], httpContext);
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid api method name.");
            }
        }

        async Task HandleApiGet(string requestString, HttpContext httpContext)
        {
            if (!_getCommands.TryGetValue(requestString, out var commandType))
            {
                SetStatusCode(httpContext.Response, 404);
                return;
            }
            var comm = (Define.IRequest)Activator.CreateInstance(commandType);
            var responseDto = await _restApi.Query(new QueryContextBase(httpContext), comm);
            //SerilazeAsJson(responseDto, httpContext);
        }

        async Task HandleArrayRequestMessages(HttpContext httpContext)
        {
            try
            {
                var bodyStr = "";
                using (var reader = new StreamReader(httpContext.Request.Body, Encoding.UTF8, true, 1024, true))
                {
                    bodyStr = reader.ReadToEnd();
                }

                using (var document = JsonDocument.Parse(bodyStr))
                {
                }
                /*
                var messages = root.ArrayObjects("Messages");
                var responseMessages = new StringBuilder("{\"Messages\":[");
                foreach (var message in messages)
                {
                    Type requestType;
                    if (!_postCommands.TryGetValue(message["Name"], out requestType))
                    {
                        SetStatusCode(response, 404);
                        return;
                    }
                    var requestObject = CreateQueryObject<Define.IRequest>(requestType, message);
                    var messageResponse = _restApi.Query(new QueryContextBase(request), requestObject).Result;
                    responseMessages.Append(JsonSerializer.SerializeToString(messageResponse, messageResponse.GetType()) + ',');
                }
                if (messages.Count > 0)
                    responseMessages.Replace(",", "]}", responseMessages.Length - 1, 1);
                else
                    responseMessages.Append("]}");
                SerilazeAsJson(responseMessages.ToString(), response);
                */
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
              //  SetStatusCodeAndWriteMessageToResponse(httpContext.Response, 500, ex.ToJson());
            }
        }

        async Task HandleApiPost(string requestString, HttpContext httpContext)
        {
          /*  try
            {
                Type requestType;
                if (!_postCommands.TryGetValue(requestString, out requestType))
                {
                    return;
                }
                var requestObject = DeserializeFromJson<object>(requestType, request);
                var responseDto = HandleApiPost(requestObject, request);
                SerilazeAsJson(responseDto, response);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }*/
        }

        Define.IResponse HandleApiPost(object requestObject, HttpContext httpContext)
        {
            if (requestObject is Define.ICommand)
            {
                var commandResult = _restApi.Command(new CommandContextBase(httpContext), (Define.ICommand)requestObject).Result;
                if (commandResult != null && commandResult.Count == 1) return commandResult[0];
                Log.Error("Response count is invalid.");
                return null;
            }
            if (requestObject is Define.IRequest request)
                return _restApi.Query(new QueryContextBase(httpContext), request).Result;
            throw new ArgumentException("Internal server error.");
        }

       /* static T CreateQueryObject<T>(Type type, JsonElement message)
        {
            var jsonProperty = message.EnumerateObject().FirstOrDefault(n => n.NameEquals("Data"));
            if (string.IsNullOrEmpty(jsonProperty.Value.GetString()))
            {
                return Activator.CreateInstance<T>();
            }
            
            //return (T)type.CreateInstance();
        }

        static T DeserializeFromJson<T>(Type requestType, IOwinRequest request)
        {
            string inputString;
            using (TextReader r = new StreamReader(request.Body, Encoding.UTF8))
            {
                inputString = r.ReadToEnd();
            }
            if (string.IsNullOrEmpty(inputString))
            {
                return (T)requestType.CreateInstance();
            }
            return (T)JsonSerializer.DeserializeFromString(inputString, requestType);
        }

        static void SerilazeAsJson(object data, HttpContext httpContext)
        {
            var stringData = JsonSerializer.SerializeToString(data, data.GetType());
            SerilazeAsJson(stringData, response);
        }

        static void SerilazeAsJson(string stringData, IOwinResponse response)
        {
            var buffer = Encoding.UTF8.GetBytes(stringData);
            response.Headers.Add("Content-Length", new[] { buffer.Length.ToString(CultureInfo.InvariantCulture) });
            response.Write(buffer);
            SetStatusCode(response, 200);
        }
       */
        void SetResponseHeaders(HttpResponse response, string[] requestPathFragments, bool allowCache = false)
        {
            var extension = Path.GetExtension(requestPathFragments.Last());
            response.ContentType = GetContentType(extension);
            if (extension == ".js" || extension == ".css")
                response.Cookies.Append("Expires",_clock.UtcNow.ToString());
            if (allowCache)
            {
                response.Cookies.Append("Expires",_clock.UtcNow.AddYears(1).ToString());//TODO
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

        static void SetNotFoundStatus(HttpResponse response)
        {
            SetStatusCode(response, 404);
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
            else
                Log.Debug($"{statusCode} {message ?? string.Empty}");
            response.StatusCode = statusCode;
        }

        static string[] GetPathFragments(string path)
        {
            var res = path.Split(new[] { @"/", @"\" }, StringSplitOptions.RemoveEmptyEntries);
            return res;
        }

    }
}