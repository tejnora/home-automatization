using Server.Core;
using Server.HttpServer;
using Server.Session;

namespace Server.Users.Queries;

[WebPost]
[Session]
public class CheckAuthorizationQuery : Define.IRequest
{
}