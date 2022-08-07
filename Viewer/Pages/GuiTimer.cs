using System;
using System.Windows.Threading;
using Hex.Logic;

namespace Viewer.Pages;

public sealed class GuiTimer : ITimer, IDisposable
{
    readonly DispatcherTimer _timer = new(DispatcherPriority.Normal);

    public GuiTimer() => _timer.Tick += TimerCallback;

    public TimeSpan Interval
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    public void Dispose() => _timer.Tick -= TimerCallback;

    public void Start() => _timer.Start();

    public void Stop() => _timer.Stop();

    public event Action OnTick;
    void TimerCallback(object _, EventArgs _2) => OnTick?.Invoke();
}