using Server.Core;
using Server.HttpServer;

namespace HttpServer.Users.Commands
{
    [WebPost]
    public class LoginCommand : Define.ICommand
    {
        public string User { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}