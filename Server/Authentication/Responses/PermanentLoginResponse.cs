using Server.Core;

namespace Server.Authentication.Responses
{
    public class PermanentLoginResponse : Define.IResponse
    {
        public ResponseType Result { get; set; }
        public string SessionId { get; set; }
        public string UserName { get; set; }
    }
}
