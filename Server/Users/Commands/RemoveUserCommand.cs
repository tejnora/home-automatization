using Server.Authentication;
using Server.Core;
using Share;

namespace Server.Users.Commands;

[WebPost]
[Session]
public class RemoveUserCommand : Define.ICommand
{
    public string Name { get; init; }
}