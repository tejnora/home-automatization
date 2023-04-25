using System;

namespace Server.Users.Responses;

public class UserListResponse
{
    public string Name { get; init; }
    public bool Enabled { get; init; }
    public DateTime LastLogin { get; init; }
}