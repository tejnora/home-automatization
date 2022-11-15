using Server.Core;
using Server.HttpServer;

namespace HttpServer.Users.Commands
{
    [WebPost]
    public class LoginByPernamentSessionIdCommand : Define.ICommand
    {
        public string PernamentSessionId { get; set; }
    }
}