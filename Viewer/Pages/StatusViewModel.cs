using Stylet;

namespace Viewer.Pages;

public class StatusViewModel : PropertyChangedBase
{
    HexCoordinate? _underMouse;

    public HexCoordinate? UnderMouse
    {
        get => _underMouse;
        set => SetAndNotify(ref _underMouse, value);
    }

    public string UnderMouseText => _underMouse?.ToString() ?? "n/a";
}