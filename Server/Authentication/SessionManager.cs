using Server.Tools;
using System.Collections.Concurrent;
using System;
using System.Linq;

namespace Server.Authentication
{
    public class SessionManager
    : ISessionManager
    {
        readonly IClock _clock;
        readonly Random _random = new Random();
        readonly ConcurrentDictionary<string, Session> _sessions = new ConcurrentDictionary<string, Session>();

        public SessionManager(IClock clock)
        {
            _clock = clock;
        }

        public Session CreateSession(string userName)
        {
            foreach (var session in _sessions)
            {
                if (session.Value.UserName != userName) continue;
                RemoveSession(session.Key);
                break;
            }
            var newSession = new Session { SessionId = CreateSessionId(256), UserName = userName, Created = _clock.UtcNow };
            _sessions[newSession.SessionId] = newSession;
            return newSession;
        }

        public void RemoveSession(string sessionId)
        {
            _sessions.TryRemove(sessionId, out _);
        }

        public bool TryGetSession(string sessionId, out Session session)
        {
            session = null;
            return !string.IsNullOrEmpty(sessionId) && _sessions.TryGetValue(sessionId, out session);
        }

        public string CreateSessionId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
