using System;

namespace Server.Tools;

class Clock
    : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
}