using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq;

namespace Hex.Logic;

public class Simulation
{
    readonly FramerateCounter _framerate;
    ImmutableDictionary<HexCoordinate, Cell> _coordinatesToCells;

    Simulation(int radius, IEnumerable<Cell> cells)
    {
        Radius = radius;
        _coordinatesToCells = cells.ToImmutableDictionary(c => c.Coordinate, c => c);
        _framerate = new FramerateCounter(new DefaultClock());
    }

    public static Simulation Create(int radius)
    {
        var rnd = new Random(1);
        var range = Enumerable.Range(-radius, 2 * radius + 1).ToArray();
        return new Simulation(radius,
            range
                .Cartesian(range, (q, r) => new HexCoordinate(q, r))
                .Where(c => Math.Abs(c.S) <= radius)
                .Select(c => new Cell(c, rnd.NextSingle())));
    }

    public Simulation Resize(int newRadius)
    {
        if (newRadius == Radius) return this;
        var result = Create(newRadius);
        foreach (var source in _coordinatesToCells.Where(c => isInResult(c.Key)))
        {
            ImmutableInterlocked.AddOrUpdate(ref result._coordinatesToCells, source.Key, source.Value,
                (_, _) => source.Value);
        }

        return result;

        bool isInResult(HexCoordinate coordinate) => result._coordinatesToCells.ContainsKey(coordinate);
    }

    public Task Run(CancellationToken ct) =>
        Task.Run(() =>
        {
            _framerate.Start();
            while (!ct.IsCancellationRequested)
            {
                Interlocked.Exchange(ref _coordinatesToCells,
                    _coordinatesToCells!.Values.AsParallel()
                        .WithDegreeOfParallelism(5)
                        .WithCancellation(ct)
                        .Select(c => c.Update())
                        .ToImmutableDictionary(c => c.Coordinate, c => c));
                _framerate.Tick();
            }
        }, ct);

    public int Framerate => _framerate.Framerate;

    public Cell[] Map => _coordinatesToCells.Values.ToArray();
    public int Radius { get; }
}

public interface IClock
{
    DateTime Now { get; }
}

public class DefaultClock : IClock
{
    public DateTime Now => DateTime.UtcNow;
}

public class FramerateCounter
{
    readonly IClock _clock;
    int _currentFramecount;
    DateTime _lastSecondStart;
    public FramerateCounter(IClock clock) => _clock = clock;

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

    public int Framerate { get; private set; }
}