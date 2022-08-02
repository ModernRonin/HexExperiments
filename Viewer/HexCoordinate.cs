using System;
using System.Windows;

namespace Viewer;

public readonly struct HexCoordinate
{
    static readonly double _rootOf3 = Math.Sqrt(3);
    public int Q { get; }
    public int R { get; }
    public int S => -Q - R;

    public HexCoordinate(int q, int r)
    {
        Q = q;
        R = r;
    }

    public Point ToPoint(double scale)
    {
        var x = scale * (_rootOf3 * Q + _rootOf3 / 2d * R);
        var y = scale * (3d       / 2d * R);
        return new Point(x, y);
    }
}