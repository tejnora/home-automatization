using Server.Core;
using Server.Core.Session;
using Server.HttpServer;

namespace HttpServer.Users.Queries
{
    [WebPost]
    [Session]
    public class CheckAuthorizationQuery : Define.IRequest
    {
    }
}