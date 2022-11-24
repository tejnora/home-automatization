using Server.Core;

namespace Server.Door.Responses;

public class DoorSettingResponse : Define.IResponse
{
    public bool Enable { get; set; }
    public string Password { get; set; }
}