using Server.Core;
using Server.Door.Queries;
using Server.Door.Responses;

namespace Server.Door;

public class DoorQueryHandler
    : Define.IQuery<DoorSettingsQuery, DoorSettingResponse>
{
    readonly DoorMqttClient _client;

    public DoorQueryHandler(DoorMqttClient client)
    {
        _client = client;
    }

    public DoorSettingResponse Consume(IQueryContext consumeContext, DoorSettingsQuery request)
    {
        return new DoorSettingResponse { Enable = true, Password = _client.Password };
    }
}