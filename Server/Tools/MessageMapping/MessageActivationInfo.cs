using System;

namespace Server.Tools.MessageMapping;

public sealed class MessageActivationInfo
{
    public Type MessageType { get; internal set; }
    public Type[] AllConsumers { get; internal set; }
}