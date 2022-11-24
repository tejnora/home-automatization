using Server.Core;
using Server.HttpServer;
using Server.Session;

namespace Server.Door.Commands;

[WebPost]
[Session]
public class UpdateDoorSettingsCommand : Define.ICommand
{
    public bool Enable { get; set; }
    public string Password { get; set; }
}