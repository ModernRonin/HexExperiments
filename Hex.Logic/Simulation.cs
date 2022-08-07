using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using MoreLinq;

namespace Hex.Logic;

public class Simulation : ISimulation
{
    readonly IFramerateCounter _framerate;
    readonly IRandomity _randomity;
    readonly IRunner _runner;
    ImmutableDictionary<HexCoordinate, Cell> _coordinatesToCells;

    public Simulation(IFramerateCounter framerate, IRandomity randomity, IRunner runner, int radius)
    {
        _framerate = framerate;
        _randomity = randomity;
        _runner = runner;
        _coordinatesToCells = create().ToImmutableDictionary(c => c.Coordinate, c => c);
        Radius = radius;

        IEnumerable<Cell> create()
        {
            var range = Enumerable.Range(-radius, 2 * radius + 1).ToArray();
            return
                range
                    .Cartesian(range, (q, r) => new HexCoordinate(q, r))
                    .Where(c => Math.Abs(c.S) <= radius)
                    .Select(c => new Cell(c, randomity.Float()));
        }
    }

    public int Framerate => _framerate.Framerate;

    public Cell[] Map => _coordinatesToCells.Values.ToArray();
    public int Radius { get; }

    public Simulation Resize(int newRadius)
    {
        if (newRadius == Radius) return this;
        var result = new Simulation(_framerate, _randomity, _runner, newRadius);
        foreach (var source in _coordinatesToCells.Where(c => isInResult(c.Key)))
        {
            ImmutableInterlocked.AddOrUpdate(ref result._coordinatesToCells, source.Key, source.Value,
                (_, _) => source.Value);
        }

        return result;

        bool isInResult(HexCoordinate coordinate) => result._coordinatesToCells.ContainsKey(coordinate);
    }

    public void Start()
    {
        _framerate.Start();
        _runner.Start(this, _framerate);
    }

    public void Stop() => _runner.Stop();

    public void Tick(CancellationToken ct)
    {
        Interlocked.Exchange(ref _coordinatesToCells,
            _coordinatesToCells!.Values.AsParallel()
                .WithDegreeOfParallelism(5)
                .WithCancellation(ct)
                .Select(c => c.Update())
                .ToImmutableDictionary(c => c.Coordinate, c => c));
    }
}