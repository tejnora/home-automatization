using Server.Authentication;
using Server.Core;
using Share;

namespace Server.Door.Commands;

[WebPost]
[Session]
public class OpenDoorCommand : Define.ICommand
{
}