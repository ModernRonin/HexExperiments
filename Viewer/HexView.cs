using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Viewer;

public class HexView : Control
{
    static HexView()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexView),
            new FrameworkPropertyMetadata(typeof(HexView)));
    }

    protected override void OnRender(DrawingContext ctx)
    {
        ctx.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 1d),
            new Rect(0, 0, RenderSize.Width, RenderSize.Height));

        foreach (var coordinate in Coordinates)
        {
            var point = coordinate.ToPoint(10) + new Vector(RenderSize.Width / 2, RenderSize.Height / 2);
            drawHex(point, 10);
        }

        void drawHex(Point center, double scale)
        {
            var pen = new Pen(Brushes.Red, 1);
            var corners = Enumerable.Range(0, 6).Select(corner).ToArray();
            for (var i = 0; i < corners.Length; i++)
            {
                var dstIndex = i < corners.Length - 1 ? i + 1 : 0;
                var (src, dst) = (corners[i], corners[dstIndex]);
                ctx.DrawLine(pen, src, dst);
            }

            Point corner(int index)
            {
                var angle = Math.PI / 3 * index - Math.PI / 6;
                return new Point(center.X + scale * Math.Cos(angle), center.Y + scale * Math.Sin(angle));
            }
        }
    }

    static IEnumerable<HexCoordinate> Coordinates
    {
        get
        {
            const int rings = 3;
            for (var q = -rings; q < rings + 1; ++q)
            {
                for (var r = -rings; r < rings + 1; ++r)
                {
                    var point = new HexCoordinate(q, r);
                    if (Math.Abs(point.S) <= rings) yield return point;
                }
            }
        }
    }
}

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