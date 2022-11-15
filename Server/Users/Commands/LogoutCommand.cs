using Server.Core;
using Server.Core.Session;
using Server.HttpServer;

namespace Server.Users.Commands
{
    [WebPost]
    [Session]
    public class LogoutCommand : Define.ICommand
    {
        public string User { get; set; }
    }
}