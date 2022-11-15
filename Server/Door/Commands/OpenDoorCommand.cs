using Server.Core;
using Server.Core.Session;
using Server.HttpServer;

namespace Server.Door.Commands
{
    [WebPost]
    [Session]
    public class OpenDoorCommand : Define.ICommand
    {
    }
}