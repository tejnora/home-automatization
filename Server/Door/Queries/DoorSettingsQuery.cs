using Server.Core;
using Server.Core.Session;
using Server.HttpServer;

namespace Server.Door.Queries
{
    [WebPost]
    [Session]
    public class DoorSettingsQuery:Define.IRequest
    {
    }
}