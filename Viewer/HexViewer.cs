using System;
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

    public static readonly DependencyProperty UnderMouseProperty =
        MakeDp(nameof(UnderMouse), typeof(HexCoordinate?));

    public static readonly DependencyProperty ScaleProperty = MakeDp(nameof(Scale), typeof(float), true);

    public static readonly DependencyProperty CellsProperty = MakeDp(nameof(Cells), typeof(Cell[]), true);

    static HexViewer() =>
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexViewer),
            new FrameworkPropertyMetadata(typeof(HexViewer)));

    public HexViewer() => Scale = 10;

    protected override void OnRender(DrawingContext ctx)
    {
        ctx.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 1d),
            new Rect(0, 0, RenderSize.Width, RenderSize.Height));

        if (Cells is null) return;

        foreach (var cell in Cells)
        {
            var point = cell.Coordinate.ToCartesian(Scale).ToPoint() + Origin;
            drawHex(point, Scale);
        }

        void drawHex(Point center, double scale)
        {
            var corners = Enumerable.Range(0, 6).Select(corner).ToArray();
            var geometry = new StreamGeometry();
            using (var geometryContext = geometry.Open())
            {
                geometryContext.BeginFigure(corners[0], true, true);
                var points = new PointCollection(corners[1..]);
                geometryContext.PolyLineTo(points, true, true);
            }

            ctx.DrawGeometry(Brushes.Green, new Pen(Brushes.Red, 1), geometry);

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

    public Cell[] Cells
    {
        get => (Cell[])GetValue(CellsProperty);
        set => SetValue(CellsProperty, value);
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

    Vector Origin => new(RenderSize.Width / 2, RenderSize.Height / 2);
}