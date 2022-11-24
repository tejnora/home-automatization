using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Serilog;
using Server.Mqtt;
using Server.Tools;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Server.Core.Mqtt;

public class MqttClientWrapper : IMqttClient
{
    readonly MqttClient _client;

    class SubTopic
    {
        public Dictionary<string, SubTopic> SubTopics { get; set; }
        public List<Action<string>> Callbacks { get; set; }

        public SubTopic()
        {
            SubTopics = new Dictionary<string, SubTopic>();
            Callbacks = new List<Action<string>>();
        }

        public void Subscribe(string topicPath, MqttClient client)
        {
            if (SubTopics.Count == 0)
            {
                client.Subscribe(new[] { topicPath }, new[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                return;
            }
            foreach (var subTopic in SubTopics)
            {
                var topic = topicPath + "/" + subTopic.Key;
                subTopic.Value.Subscribe(topic, client);
            }
        }

        public void Publish(IList<string> topic, int topicIndex, string payload)
        {
            if (topicIndex == topic.Count)
            {
                foreach (var callback in Callbacks)
                {
                    callback(payload);
                }
                return;
            }
            var currentTopis = topic[topicIndex];
            if (!SubTopics.TryGetValue(topic[topicIndex], out var subTopic))
            {
                Debug.Assert(false);
                return;
            }
            subTopic.Publish(topic, ++topicIndex, payload);
        }
    }

    readonly SubTopic _subscriptions = new SubTopic();
    private ReaderWriterLockSlim _topicLockSlim = new ReaderWriterLockSlim();

    public MqttClientWrapper(ServerOptions _config)
    {
        _client = new MqttClient(IPAddress.Parse(_config.MqttClientAddress));
        _client.MqttMsgPublishReceived += OnPublishArrived;
        _client.ConnectionClosed += OnConnectionLost;
        Connect();
    }

    void Connect()
    {
        if (_client.IsConnected) return;
        try
        {
            _client.Connect("Server");
            RegisterOurSubscriptions();

        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());
        }
    }

    void Disconnect()
    {
        if (_client.IsConnected)
        {
            Console.WriteLine("Client disconnecting\n");
            _client.Disconnect();
            Console.WriteLine("Client disconnected\n");
        }
    }

    void OnConnectionLost(object sender, EventArgs e)
    {
    }

    void RegisterOurSubscriptions()
    {
        using (_topicLockSlim.WriteLock())
        {
            if (_subscriptions.SubTopics.Count == 0)
                return;
            _subscriptions.Subscribe("", _client);
        }
    }

    void OnPublishArrived(object sender, MqttMsgPublishEventArgs e)
    {
        using (_topicLockSlim.ReadLock())
        {
            var topic = e.Topic.Substring(1).Split('/');
            _subscriptions.Publish(topic, 0, e.Topic);
        }
    }

    public void Subscribe(string topic, Action<string> callBack)
    {
        var items = topic.Split('/').Skip(1);
        using (_topicLockSlim.WriteLock())
        {
            var currentTopic = _subscriptions;
            var subscribe = false;
            foreach (var item in items)
            {
                SubTopic subTopic;
                if (!currentTopic.SubTopics.TryGetValue(item, out subTopic))
                {
                    subTopic = new SubTopic();
                    currentTopic.SubTopics[item] = subTopic;
                    subscribe = true;
                }
                currentTopic = subTopic;
                if (item == "#") break;
            }
            currentTopic.Callbacks.Add(callBack);
            if (subscribe)
            {
                _client.Subscribe(new[] { topic }, new[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
            }
        }
    }

    public bool Publish(string topic, string paiload)
    {
        if (!_client.IsConnected)
        {
            Connect();
            if (!_client.IsConnected) return false;
        }
        try
        {
            _client.Publish(topic, Encoding.UTF8.GetBytes(paiload), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex.ToString());

        }
        return false;
    }
}