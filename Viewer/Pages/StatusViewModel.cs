using Hex.Logic;
using PropertyChanged.SourceGenerator;

namespace Viewer.Pages;

public partial class StatusViewModel
{
    [Notify] int _framerate;
    [Notify] HexCoordinate? _underMouse;
    [Notify] float _zoom;
    public string FramerateText => $"{Framerate} FPS";
    public string UnderMouseText => UnderMouse?.ToString() ?? "n/a";
}