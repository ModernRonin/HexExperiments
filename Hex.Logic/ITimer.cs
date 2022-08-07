using System;

namespace Hex.Logic;

public interface ITimer : IStartStoppable
{
    TimeSpan Interval { get; set; }
    event Action OnTick;
}