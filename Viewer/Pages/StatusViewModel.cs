namespace Viewer.Pages;

public class StatusViewModel
{
    [Notify] HexCoordinate? _underMouse;
    public string UnderMouseText => UnderMouse?.ToString() ?? "n/a";
}