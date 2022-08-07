using System;

namespace Hex.Logic;

public class FramerateCounter : IFramerateCounter
{
    readonly IClock _clock;
    int _currentFramecount;
    DateTime _lastSecondStart;
    public FramerateCounter(IClock clock) => _clock = clock;

    public int Framerate { get; private set; }

    public void Start()
    {
        _lastSecondStart = _clock.Now;
        _currentFramecount = Framerate = 0;
    }

    public void Tick()
    {
        var now = _clock.Now;
        if (now - _lastSecondStart < TimeSpan.FromSeconds(1)) ++_currentFramecount;
        else
        {
            Framerate = _currentFramecount;
            _lastSecondStart = now;
            _currentFramecount = 1;
        }
    }
}