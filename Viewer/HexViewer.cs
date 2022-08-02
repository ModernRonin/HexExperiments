using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Hex.Logic;

namespace Viewer;

public class HexViewer : Control
{
    const double SixthOfPi = Math.PI / 6;

    public static readonly DependencyProperty
        RingCountProperty = MakeDp(nameof(RingCount), typeof(int), true);

    public static readonly DependencyProperty UnderMouseProperty =
        MakeDp(nameof(UnderMouse), typeof(HexCoordinate?));

    public static readonly DependencyProperty ScaleProperty = MakeDp(nameof(Scale), typeof(float), true);

    static HexViewer() =>
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexViewer),
            new FrameworkPropertyMetadata(typeof(HexViewer)));

    public HexViewer() => Scale = 10;

    protected override void OnRender(DrawingContext ctx)
    {
        ctx.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 1d),
            new Rect(0, 0, RenderSize.Width, RenderSize.Height));

        foreach (var coordinate in Coordinates)
        {
            var point = coordinate.ToCartesian(Scale).ToPoint() + Origin;
            drawHex(point, Scale);
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

    protected override void OnMouseLeave(MouseEventArgs e) => UnderMouse = null;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        var pixelPosition = e.GetPosition(this) - Origin;
        UnderMouse = HexCoordinate.FromCartesian(pixelPosition.ToVector(), Scale);
    }

    public int RingCount
    {
        get => (int)GetValue(RingCountProperty);
        set => SetValue(RingCountProperty, value);
    }

    public float Scale
    {
        get => (float)GetValue(ScaleProperty);
        set => SetValue(ScaleProperty, value);
    }

    public HexCoordinate? UnderMouse
    {
        get => (HexCoordinate?)GetValue(UnderMouseProperty);
        set => SetValue(UnderMouseProperty, value);
    }

    static DependencyProperty MakeDp(string name, Type type, bool doTriggerRenderOnChange = false)
    {
        var metadata = doTriggerRenderOnChange
            ? new FrameworkPropertyMetadata(type.DefaultValue(),
                FrameworkPropertyMetadataOptions.AffectsRender)
            : new PropertyMetadata(type.DefaultValue());
        return DependencyProperty.Register(name, type, typeof(HexViewer), metadata);
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

    Vector Origin => new(RenderSize.Width / 2, RenderSize.Height / 2);
}