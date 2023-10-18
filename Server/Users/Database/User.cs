using BTDB.ODBLayer;
using System;

namespace Server.Users.Database;

public class User
{
    [PrimaryKey(1)]
    public string Name { get; set; }
    public string Password { get; set; }
    public byte[] Salt { get; set; }
    public string PermanentSessionId { get; set; }
    public bool Enabled { get; set; }
    public DateTime LastLogin { get; set; }
}