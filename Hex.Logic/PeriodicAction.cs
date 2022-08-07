using System;

namespace Hex.Logic;

public sealed class PeriodicAction : IStartStoppable, IDisposable
{
    readonly Action _action;
    readonly ITimer _timer;

    public PeriodicAction(ITimer timer, Action action, int updatesPerSecond)
    {
        _timer = timer;
        _action = action;
        _timer.Interval = TimeSpan.FromSeconds(1d / updatesPerSecond);
        _timer.OnTick += _action;
    }

    public void Dispose() => _timer.OnTick -= _action;

    public void Start() => _timer.Start();

    public void Stop() => _timer.Stop();
}