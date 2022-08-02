using System;
using System.Windows;
using static System.Math;

namespace Viewer;

public readonly struct HexCoordinate
{
    static readonly double _rootOf3 = Sqrt(3);
    public int Q { get; }
    public int R { get; }
    public int S => -Q - R;

    public HexCoordinate(int q, int r)
    {
        Q = q;
        R = r;
    }

    public static HexCoordinate FromPoint(Point point, double scale)
    {
        var q = (_rootOf3 / 3 * point.X - 1d / 3 * point.Y) / scale;
        var r = 2d / 3 * point.Y                            / scale;
        return Round(q, r);
    }

    public Point ToPoint(double scale)
    {
        var x = scale * (_rootOf3 * Q + _rootOf3 / 2d * R);
        var y = scale * (3d       / 2d * R);
        return new Point(x, y);
    }

    static HexCoordinate Round(double q, double r)
    {
        var s = -q - r;
        var (fixedQ, fixedR, fixedS) = (Math.Round(q), Math.Round(r), Math.Round(s));
        var (deltaQ, deltaR, deltaS) = (Abs(fixedQ - q), Abs(fixedR - r), Abs(fixedS - s));
        if (deltaQ > deltaR && deltaQ > deltaS) q = -r - s;
        else if (deltaR > deltaS) r = -q               - s;

        return new HexCoordinate((int)q, (int)r);
    }

    public override string ToString() => $"({Q}/{R}/{S})";
}