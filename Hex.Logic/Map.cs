using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MoreLinq;

namespace Hex.Logic;

public class Map
{
    ImmutableDictionary<HexCoordinate, Cell> _cells;

    Map(int radius, IEnumerable<Cell> cells)
    {
        Radius = radius;
        _cells = cells.ToImmutableDictionary(c => c.Coordinate, c => c);
    }

    public static Map Create(int radius)
    {
        var rnd = new Random(1);
        var range = Enumerable.Range(-radius, 2 * radius + 1).ToArray();
        return new Map(radius,
            range
                .Cartesian(range, (q, r) => new HexCoordinate(q, r))
                .Where(c => Math.Abs(c.S) <= radius)
                .Select(c => new Cell(c, rnd.NextSingle())));
    }

    public Map Resize(int newRadius)
    {
        if (newRadius == Radius) return this;
        var result = Create(newRadius);
        foreach (var source in _cells.Where(c => isInResult(c.Key)))
        {
            ImmutableInterlocked.AddOrUpdate(ref result._cells, source.Key, source.Value,
                (_, _) => source.Value);
        }

        return result;

        bool isInResult(HexCoordinate coordinate) => result._cells.ContainsKey(coordinate);
    }

    public Task Run(CancellationToken ct) =>
        Task.Run(() =>
        {
            while (!ct.IsCancellationRequested)
            {
                Interlocked.Exchange(ref _cells,
                    _cells.Values.AsParallel()
                        .WithDegreeOfParallelism(5)
                        .WithCancellation(ct)
                        .Select(c => c.Update())
                        .ToImmutableDictionary(c => c.Coordinate, c => c));
            }
        }, ct);

    public Cell[] Cells => _cells.Values.ToArray();
    public int Radius { get; }
}