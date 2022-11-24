using Server.Core;
using Server.HttpServer;
using Server.Session;

namespace Server.Door.Queries;

[WebPost]
[Session]
public class DoorSettingsQuery:Define.IRequest
{
}