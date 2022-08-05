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

    public static readonly DependencyProperty CellsProperty = MakeDp(nameof(Cells), typeof(Cell[]), true);

    float _scale;

    HexCoordinate? _underMouse;

    static HexViewer() =>
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexViewer),
            new FrameworkPropertyMetadata(typeof(HexViewer)));

    public HexViewer() => Scale = 10f;

    protected override void OnRender(DrawingContext ctx)
    {
        ctx.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 1d),
            new Rect(0, 0, RenderSize.Width, RenderSize.Height));

        if (Cells is null) return;

        foreach (var cell in Cells)
        {
            var point = cell.Coordinate.ToCartesian(Scale).ToPoint() + Origin;
            drawHex(point, cell.Amount);
        }

        void drawHex(Point center, float value)
        {
            var corners = Enumerable.Range(0, 6).Select(corner).ToArray();
            var geometry = new StreamGeometry();
            using (var geometryContext = geometry.Open())
            {
                geometryContext.BeginFigure(corners[0], true, true);
                var points = new PointCollection(corners[1..]);
                geometryContext.PolyLineTo(points, true, true);
            }

            var fillColor = new SolidColorBrush(new Color
            {
                ScG = value,
                A = 255
            });
            ctx.DrawGeometry(fillColor, new Pen(Brushes.Red, 1), geometry);

            Point corner(int index)
            {
                var angle = SixthOfPi * (2 * index - 1);
                return new Point(center.X + Scale * Math.Cos(angle), center.Y + Scale * Math.Sin(angle));
            }
        }
    }

    protected override void OnMouseLeave(MouseEventArgs e) => UnderMouse = null;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        var pixelPosition = e.GetPosition(this) - Origin;
        var coordinate = HexCoordinate.FromCartesian(pixelPosition.ToVector(), Scale);
        UnderMouse = Cells.Select(c => c.Coordinate).Contains(coordinate) ? coordinate : null;
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        var factor = e.Delta switch
        {
            < 0 => 0.9f,
            > 1 => 1.1f,
            _ => 1f
        };
        Scale *= factor;
    }

    public event EventHandler<EventArgs<float>> ScaleChanged;

    public event EventHandler<EventArgs<HexCoordinate?>> UnderMouseChanged;

    public Cell[] Cells
    {
        get => (Cell[])GetValue(CellsProperty);
        set => SetValue(CellsProperty, value);
    }

    public float Scale
    {
        get => _scale;
        set
        {
            _scale = value;
            ScaleChanged?.Invoke(this, _scale);
            InvalidateVisual();
        }
    }

    public HexCoordinate? UnderMouse
    {
        get => _underMouse;
        set
        {
            _underMouse = value;
            UnderMouseChanged?.Invoke(this, _underMouse);
        }
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