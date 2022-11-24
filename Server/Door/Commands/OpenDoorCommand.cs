using Server.Core;
using Server.HttpServer;
using Server.Session;

namespace Server.Door.Commands;

[WebPost]
[Session]
public class OpenDoorCommand : Define.ICommand
{
}