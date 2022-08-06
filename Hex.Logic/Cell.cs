namespace Hex.Logic;

public readonly record struct Cell(HexCoordinate Coordinate, float Amount)
{
    public Cell Update()
    {
        var newAmount = Amount + 0.0001f;
        if (newAmount >= 1f) newAmount -= 1f;
        return this with { Amount = newAmount };
    }
}