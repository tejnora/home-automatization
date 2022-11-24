using Server.Core;
using Server.HttpServer;

namespace Server.Users.Commands;

[WebPost]
public class LoginByPernamentSessionIdCommand : Define.ICommand
{
    public string PernamentSessionId { get; set; }
}