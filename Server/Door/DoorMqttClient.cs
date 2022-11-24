using System.Diagnostics;
using Server.Core.Mqtt;
using Server.Mqtt;

namespace Server.Door;

public class DoorMqttClient
{
    readonly IMqttClient _mqttClient;

    public DoorMqttClient(IMqttClient mqttClient)
    {
        _mqttClient = mqttClient;
        _mqttClient.Subscribe("/d/h", (s =>
        {
            Debug.Write(s);
        }));
        _mqttClient.Subscribe("/d/p", (s =>
        {
            Password = s;
        }));
    }

    public string Password { get; private set; }

    public bool OpenDoor()
    {
        return _mqttClient.Publish("/d/o", "");
    }

}