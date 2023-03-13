using Server.Authentication;
using Server.Core;
using Server.HttpServer;
using Share;

namespace Server.Door.Queries;

[WebPost]
[Session]
public class DoorSettingsQuery:Define.IRequest
{
}