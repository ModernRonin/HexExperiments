using MoreLinq;

namespace Hex.Logic;

public class Map
{
    Map(IEnumerable<Cell> cells) => Cells = cells.ToArray();

    public static Map Create(int radius)
    {
        var rnd = new Random(1);
        var range = Enumerable.Range(-radius, 2 * radius).ToArray();
        return new Map(range
            .Cartesian(range, (q, r) => new HexCoordinate(q, r))
            .Where(c => Math.Abs(c.S) <= radius)
            .Select(c => new Cell(c, rnd.NextSingle())));
    }

    public Cell[] Cells { get; }
}