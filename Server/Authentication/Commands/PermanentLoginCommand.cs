using Server.Core;
using Server.HttpServer;
using Share;

namespace Server.Authentication.Commands;

[WebPost]
public class PermanentLoginCommand : Define.ICommand
{
    public string Name { get; set; }
    public string Token{ get; set; }
}