using System;

namespace Server.Authentication;

public class Session
{
    public string SessionId;
    public string UserName;
    public DateTime Created;
}