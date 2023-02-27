using Server.Core;

namespace Server.Authentication.Responses;

public class LoginResponse : Define.IResponse
{
    public ResponseType Result { get; set; }
    public string SessionId { get; set; }
    public string PermanentSessionId { get; set; }
}