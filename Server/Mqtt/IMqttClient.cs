using System;

namespace Server.Mqtt;

public interface IMqttClient
{
    bool Publish(string topic, string paiload);
    void Subscribe(string topic, Action<string> callBack);
}