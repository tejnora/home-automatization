using Server.Core;
using Share;

namespace Server.Authentication.Commands;

[WebPost]
public class LoginCommand : Define.ICommand
{
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}