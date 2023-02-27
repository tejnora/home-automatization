using Server.Core;
using Server.HttpServer;

namespace Server.Authentication.Commands;

[WebPost]
public class LoginCommand : Define.ICommand
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}