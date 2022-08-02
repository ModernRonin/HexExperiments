using System.Numerics;
using static System.Math;

namespace Hex.Logic;

public readonly record struct HexCoordinate(int Q, int R)
{
    static readonly float _rootOf3 = (float)Sqrt(3);

    public override string ToString() => $"({Q}/{R}/{S})";

    public static HexCoordinate FromPoint(Vector2 point, float scale)
    {
        var q = (_rootOf3 / 3 * point.X - 1f / 3 * point.Y) / scale;
        var r = 2f / 3 * point.Y                            / scale;
        return Clamp(q, r);
    }

    public Vector2 ToPoint(float scale)
    {
        var x = scale * (_rootOf3 * Q + _rootOf3 / 2f * R);
        var y = scale * (3f       / 2f * R);
        return new Vector2(x, y);
    }

    public int S => -Q - R;

    static HexCoordinate Clamp(float q, float r)
    {
        var s = -q - r;
        var (fixedQ, fixedR, fixedS) = (round(q), round(r), round(s));
        var (deltaQ, deltaR, deltaS) = (Abs(fixedQ - q), Abs(fixedR - r), Abs(fixedS - s));
        if (deltaQ > deltaR && deltaQ > deltaS) q = -r - s;
        else if (deltaR > deltaS) r = -q               - s;

        return new HexCoordinate((int)q, (int)r);

        float round(float what) => (float)Round(what);
    }
}