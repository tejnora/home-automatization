using Microsoft.AspNetCore.Http;

namespace Server.Core;

public class QueryContextBase : IQueryContext
{
    public QueryContextBase(HttpContext httpContext)
    {
        SessionId = httpContext.Request.Cookies["session-id"];
    }
    public string SessionId { get; }
}