using BTDB.ODBLayer;
using Microsoft.AspNetCore.Http;

namespace Server.Core;

public class CommandContextBase : ICommandContext
{
    public CommandContextBase(HttpContext httpContext)
    {
        SessionId = httpContext?.Request.Cookies["session-id"];
    }
    public string SessionId { get; }
    public IObjectDBTransaction Transaction { get; set; }

    public T Table<T>() where T : class, IRelation
    {
        return Transaction.GetRelation<T>();
    }
}
