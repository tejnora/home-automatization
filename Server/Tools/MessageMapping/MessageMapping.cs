using System;

namespace Server.Tools.MessageMapping;

public sealed class MessageMapping
{
    public readonly Type Consumer;
    public readonly Type Message;

    public MessageMapping(Type consumer, Type message)
    {
        Consumer = consumer;
        Message = message;
    }

    public override string ToString()
    {
        return $"{Consumer.Name} ==> {Message.Name}";
    }

    public bool Equals(MessageMapping other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return other.Consumer == Consumer && other.Message == Message;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != typeof(MessageMapping)) return false;
        return Equals((MessageMapping)obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var result = (Consumer != null ? Consumer.GetHashCode() : 0);
            result = (result * 397) ^ (Message != null ? Message.GetHashCode() : 0);
            return result;
        }
    }
}