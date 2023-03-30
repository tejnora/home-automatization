using BTDB.ODBLayer;

namespace Server.Core;

public interface ICommandContext
{
    string SessionId { get; }
    IObjectDBTransaction Transaction { get; set; }
    T Table<T>() where T : class, IRelation;
}