using Hex.Logic;
using PropertyChanged.SourceGenerator;

namespace Viewer.Pages;

public partial class StatusViewModel
{
    [Notify] HexCoordinate? _underMouse;
    [Notify] float _zoom;
    public string UnderMouseText => UnderMouse?.ToString() ?? "n/a";
}