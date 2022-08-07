using System;
using System.Threading;
using System.Threading.Tasks;

namespace Hex.Logic;

public sealed class Runner : IRunner, IDisposable
{
    CancellationTokenSource _cancellation;
    Task _task;

    public void Dispose()
    {
        Stop();
        _cancellation?.Dispose();
    }

    public void Start(params ITickable[] tickables)
    {
        _cancellation = new CancellationTokenSource();
        _task = Task.Run(() =>
        {
            while (!_cancellation.IsCancellationRequested)
                foreach (var tickable in tickables)
                    tickable.Tick(_cancellation.Token);
        });
    }

    public void Stop()
    {
        _cancellation?.Cancel();
        _cancellation = null;
    }
}