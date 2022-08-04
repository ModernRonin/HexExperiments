using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace Hex.Logic;

public class Map
{
    readonly Dictionary<HexCoordinate, Cell> _cells;

    Map(int radius, IEnumerable<Cell> cells)
    {
        Radius = radius;
        _cells = cells.ToDictionary(c => c.Coordinate, c => c);
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
        foreach (var source in _cells.Where(c => isInResult(c.Key))) result._cells[source.Key] = source.Value;
        return result;

        bool isInResult(HexCoordinate coordinate) => result._cells.ContainsKey(coordinate);
    }

    public Cell[] Cells => _cells.Values.ToArray();
    public int Radius { get; }
}