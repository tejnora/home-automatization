using Server.Authentication;
using Server.Core;
using Share;

namespace Server.Users.Commands;

[WebPost]
[Session]
public class UpdateUserCommand : Define.ICommand
{
    public string Name { get; init; }
    public bool Enabled { get; init; }
}