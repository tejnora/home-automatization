using System;

namespace Server.Core.MessageMapping
{
    public sealed class MessageActivationInfo
    {
        public Type MessageType { get; internal set; }
        public Type[] AllConsumers { get; internal set; }
    }
}