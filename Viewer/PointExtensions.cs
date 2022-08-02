using System.Numerics;
using System.Windows;

namespace Viewer;

public static class PointExtensions
{
    public static Point ToPoint(this Vector2 self) => new(self.X, self.Y);
    public static Vector2 ToVector(this Point self) => new((float)self.X, (float)self.Y);
}