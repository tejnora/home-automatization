using Server.Core;

namespace Server.Users.Responses
{
    public class LoginResponse : Define.IResponse
    {
        public ResponseType Result { get; set; }
        public string SessionId { get; set; }
        public string PernamentSessionId { get; set; }
    }
}