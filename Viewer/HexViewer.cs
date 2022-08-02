using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Viewer;

public class HexViewer : Control
{
    const double SixthOfPi = Math.PI / 6;

    public static readonly DependencyProperty RingCountProperty = DependencyProperty.Register("RingCount",
        typeof(int), typeof(HexViewer),
        new FrameworkPropertyMetadata(default(int), FrameworkPropertyMetadataOptions.AffectsRender));

    static HexViewer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexViewer),
            new FrameworkPropertyMetadata(typeof(HexViewer)));
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
                var angle = SixthOfPi * (2 * index - 1);
                return new Point(center.X + scale * Math.Cos(angle), center.Y + scale * Math.Sin(angle));
            }
        }
    }

    public int RingCount
    {
        get => (int)GetValue(RingCountProperty);
        set => SetValue(RingCountProperty, value);
    }

    IEnumerable<HexCoordinate> Coordinates
    {
        get
        {
            for (var q = -RingCount; q < RingCount + 1; ++q)
            {
                for (var r = -RingCount; r < RingCount + 1; ++r)
                {
                    var point = new HexCoordinate(q, r);
                    if (Math.Abs(point.S) <= RingCount) yield return point;
                }
            }
        }
    }
}