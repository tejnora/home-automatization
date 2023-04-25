using Server.Authentication;
using Server.Core;
using Share;

namespace Server.Users.Commands;

[WebPost]
[Session]
public class UserChangePasswordCommand : Define.ICommand
{
    public string User { get; set; }
    public string NewPassword { get; set; }
    public string OriginPassword { get; set; }
}