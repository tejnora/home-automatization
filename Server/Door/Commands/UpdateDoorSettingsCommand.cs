﻿using Server.Authentication;
using Server.Core;
using Server.HttpServer;
using Share;

namespace Server.Door.Commands;

[WebPost]
[Session]
public class UpdateDoorSettingsCommand : Define.ICommand
{
    public bool Enable { get; set; }
    public string Password { get; set; }
}