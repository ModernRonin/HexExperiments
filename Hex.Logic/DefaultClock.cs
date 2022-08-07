using System;

namespace Hex.Logic;

public class DefaultClock : IClock
{
    public DateTime Now => DateTime.UtcNow;
}