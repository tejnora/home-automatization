using BTDB.ODBLayer;
using Microsoft.AspNetCore.Http;
using Server.Users.Database;

namespace Server.Core;

public class CommandContextBase : ICommandContext
{
    public CommandContextBase(HttpContext httpContext)
    {
        SessionId = httpContext?.Request.Cookies["session-id"];
    }
    public string SessionId { get; }
    public IObjectDBTransaction Transaction { get; set; }

    public T Table<T>(string tableName) where T : class, IRelation
    {
        var creator = Transaction.InitRelation<T>("Users");
        var personTable = creator(Transaction);
        return personTable;
    }
}
