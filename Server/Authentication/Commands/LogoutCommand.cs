using Server.Core;
using Server.HttpServer;
using Share;

namespace Server.Authentication.Commands;

[WebPost]
[Session]
public class LogoutCommand : Define.ICommand
{
    public string User { get; set; }
}