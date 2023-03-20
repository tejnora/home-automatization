using Server.Core;
using Share;

namespace Server.Users.Commands;

[WebPost]
public class CreateUserCommand : Define.ICommand
{
    public string Name { get; init; }
    public string Password { get; init; }
}
