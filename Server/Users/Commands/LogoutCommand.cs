using Server.Core;
using Server.HttpServer;
using Server.Session;

namespace Server.Users.Commands;

[WebPost]
[Session]
public class LogoutCommand : Define.ICommand
{
    public string User { get; set; }
}