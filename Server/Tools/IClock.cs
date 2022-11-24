using System;

namespace Server.Tools;

public interface IClock
{
    DateTime UtcNow { get; }
}