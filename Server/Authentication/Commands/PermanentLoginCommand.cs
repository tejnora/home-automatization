using Server.Core;
using Server.HttpServer;

namespace Server.Authentication.Commands;

[WebPost]
public class PermanentLoginCommand : Define.ICommand
{
    public string Token{ get; set; }
}