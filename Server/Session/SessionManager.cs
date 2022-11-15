using System;
using System.Collections.Concurrent;
using System.Linq;
using Server.Tools;

namespace Server.Core.Session
{
    public class SessionManager
        : ISessionManager
    {
        readonly IClock _clock;
        readonly Random _random = new Random();
        readonly ConcurrentDictionary<string, Server.Core.Session.Session> _sessions = new ConcurrentDictionary<string, Server.Core.Session.Session>();
        public SessionManager(IClock clock)
        {
            _clock = clock;
        }

        public Server.Core.Session.Session CreateSession(string userName)
        {
            foreach (var session in _sessions)
            {
                if (session.Value.UserName != userName) continue;
                RemoveSession(session.Key);
                break;
            }
            var newSession = new Server.Core.Session.Session { SessionId = CreateSessionId(), UserName = userName, Created = _clock.UtcNow };
            _sessions[newSession.SessionId] = newSession;
            return newSession;
        }

        public void RemoveSession(string sessionId)
        {
            Server.Core.Session.Session session;
            _sessions.TryRemove(sessionId, out session);
        }

        public bool TryGetSession(string sessionId, out Server.Core.Session.Session session)
        {
            session = null;
            return !string.IsNullOrEmpty(sessionId) && _sessions.TryGetValue(sessionId, out session);
        }

        public string CreateSessionId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, 30)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

    }
}