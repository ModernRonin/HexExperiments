using System;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Hex.Logic;

namespace Viewer;

public class HexViewer : Control
{
    public static readonly DependencyProperty CellsProperty = MakeDp(nameof(Cells), typeof(Cell[]), true);

    public static readonly DependencyProperty MaximumZoomProperty =
        MakeDp(nameof(MaximumZoom), typeof(float), true);

    static readonly Vector2[] _directionVectors;

    bool _isPanning;
    Vector2 _origin;
    Vector2 _panOrigin;
    HexCoordinate? _underMouse;
    float _zoom;

    static HexViewer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(HexViewer),
            new FrameworkPropertyMetadata(typeof(HexViewer)));

        _directionVectors = Enumerable.Range(0, 6).Select(toVector).ToArray();

        Vector2 toVector(int index)
        {
            const double sixthOfPi = Math.PI / 6;
            var angle = sixthOfPi * (2 * index - 1);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
    }

    public HexViewer() => Zoom = MaximumZoom = 10f;

    protected override void OnRender(DrawingContext ctx)
    {
        ctx.DrawRectangle(Brushes.Black, new Pen(Brushes.Black, 1d),
            new Rect(0, 0, RenderSize.Width, RenderSize.Height));

        if (Cells is null) return;

        foreach (var cell in Cells)
        {
            var point = cell.Coordinate.ToCartesian(Zoom) + Origin;
            drawHex(point, cell.Amount);
        }

        void drawHex(Vector2 center, float value)
        {
            var corners = _directionVectors.Select(d => center + d * Zoom)
                .Select(c => c.ToPoint())
                .ToArray();
            if (corners.All(isOutsideViewport)) return;

            var geometry = getOutline(corners);
            var fillColor = new SolidColorBrush(new Color
            {
                ScG = value,
                A = 255
            });
            ctx.DrawGeometry(fillColor, new Pen(Brushes.Red, 0), geometry);

            bool isOutsideViewport(Point point)
            {
                if (point.X < 0) return true;
                if (point.Y < 0) return true;
                if (point.X > RenderSize.Width) return true;
                if (point.Y > RenderSize.Height) return true;
                return false;
            }
        }

        Geometry getOutline(Point[] points)
        {
            var result = new StreamGeometry();
            using var geometryContext = result.Open();
            geometryContext.BeginFigure(points[0], true, true);
            geometryContext.PolyLineTo(new PointCollection(points[1..]), true, true);
            return result;
        }
    }

    protected override void OnMouseLeave(MouseEventArgs e) => UnderMouse = null;

    protected override void OnMouseMove(MouseEventArgs e)
    {
        var pixelPosition = e.GetPosition(this).ToVector();
        if (_isPanning)
        {
            var current = pixelPosition;
            var delta = current - _panOrigin;
            _panOrigin = current;
            Origin += delta;
        }
        else
        {
            var adjustedToOrigin = pixelPosition - Origin;
            var coordinate = HexCoordinate.FromCartesian(adjustedToOrigin, Zoom);
            UnderMouse = Cells.Select(c => c.Coordinate).Contains(coordinate) ? coordinate : null;
        }
    }

    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        _panOrigin = e.GetPosition(this).ToVector();
        _isPanning = true;
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) => _isPanning = false;

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
        var factor = e.Delta switch
        {
            < 0 => 0.9f,
            > 1 => 1.1f,
            _ => 1f
        };
        Zoom *= factor;
        Zoom = Math.Min(Zoom, MaximumZoom);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
        Origin = new Vector2((float)(sizeInfo.NewSize.Width / 2), (float)(sizeInfo.NewSize.Height / 2));
    }

    public event EventHandler<EventArgs<HexCoordinate?>> UnderMouseChanged;

    public event EventHandler<EventArgs<float>> ZoomChanged;

    public Cell[] Cells
    {
        get => (Cell[])GetValue(CellsProperty);
        set => SetValue(CellsProperty, value);
    }

    public float MaximumZoom
    {
        get => (float)GetValue(MaximumZoomProperty);
        set => SetValue(MaximumZoomProperty, value);
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

    public float Zoom
    {
        get => _zoom;
        set
        {
            _zoom = value;
            ZoomChanged?.Invoke(this, _zoom);
            InvalidateVisual();
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

    Vector2 Origin
    {
        get => _origin;
        set
        {
            _origin = value;
            InvalidateVisual();
        }
    }
}