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
    ImmutableDictionary<HexCoordinate, Cell> _coordinatesToCells;

    Simulation(int radius, IEnumerable<Cell> cells)
    {
        Radius = radius;
        _coordinatesToCells = cells.ToImmutableDictionary(c => c.Coordinate, c => c);
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
            Framerate = 0;
            var started = DateTime.UtcNow;
            while (!ct.IsCancellationRequested)
            {
                Interlocked.Exchange(ref _coordinatesToCells,
                    _coordinatesToCells!.Values.AsParallel()
                        .WithDegreeOfParallelism(5)
                        .WithCancellation(ct)
                        .Select(c => c.Update())
                        .ToImmutableDictionary(c => c.Coordinate, c => c));
                var now = DateTime.UtcNow;
                if (now - started > TimeSpan.FromSeconds(1))
                {
                    Framerate = 1;
                    started = now;
                }
                else ++Framerate;
            }
        }, ct);

    public int Framerate { get; private set; }

    public Cell[] Map => _coordinatesToCells.Values.ToArray();
    public int Radius { get; }
}