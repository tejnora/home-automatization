using Server.Core;
using Share;

namespace Server.Users.Commands;

[WebPost]
public class RemoveUserCommand : Define.ICommand
{
    public string Name { get; init; }
}