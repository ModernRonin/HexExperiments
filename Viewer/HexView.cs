using System;
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

        drawHex(new Point(RenderSize.Width / 2, RenderSize.Height / 2), 10);

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
}