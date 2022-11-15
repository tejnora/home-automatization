using HttpServer.Door;
using Server.Core;
using Server.Door.Commands;

namespace Server.Door
{
    public class DoorCommandHandler
        : Define.IConsumer<UpdateDoorSettingsCommand, GeneralResponses>
        , Define.IConsumer<OpenDoorCommand, GeneralResponses>
    {
        readonly DoorMqttClient _mqttClient;

        public DoorCommandHandler(DoorMqttClient mqttClient)
        {
            _mqttClient = mqttClient;
        }
        public GeneralResponses Consume(ICommandContext context, UpdateDoorSettingsCommand command)
        {
            return GeneralResponses.Success;
        }

        public GeneralResponses Consume(ICommandContext context, OpenDoorCommand command)
        {
            _mqttClient.OpenDoor();
            return GeneralResponses.Success;
        }

    }
}