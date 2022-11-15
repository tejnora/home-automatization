using HttpServer.Door;
using Server.Core;
using Server.Door.Queries;
using Server.Door.Responses;

namespace Server.Door
{
    public class DoorQueryHandler
        : Define.IQuery<DoorSettingsQuery, DoorSettingRespone>
    {
        readonly DoorMqttClient _client;

        public DoorQueryHandler(DoorMqttClient client)
        {
            _client = client;
        }

        public DoorSettingRespone Consume(IQueryContext consumeContext, DoorSettingsQuery request)
        {
            return new DoorSettingRespone { Enable = true, Password = _client.Password };
        }
    }
}