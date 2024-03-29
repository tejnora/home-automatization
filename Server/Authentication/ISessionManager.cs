﻿namespace Server.Authentication;

public interface ISessionManager
{
    Session CreateSession(string userName);
    void RemoveSession(string sessionId);
    bool TryGetSession(string sessionId, out Session session);
    string CreateSessionId(int length);
}