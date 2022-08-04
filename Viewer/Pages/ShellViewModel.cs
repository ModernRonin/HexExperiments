using Hex.Logic;
using PropertyChanged.SourceGenerator;
using Stylet;

namespace Viewer.Pages;

public partial class ShellViewModel : Screen
{
    readonly HexConfigurationViewModel _configuration = new();
    readonly IWindowManager _windowManager;
    [Notify] Map _map;

    public ShellViewModel(IWindowManager windowManager)
    {
        _windowManager = windowManager;
        Map = Map.Create(_configuration.RingCount);
    }

    public void ConfigureMap()
    {
        _configuration.RingCount = Map.Radius;
        if (_windowManager.ShowDialog(_configuration).GetValueOrDefault())
            Map = Map.Resize(_configuration.RingCount);
    }

    public Cell[] Cells => Map.Cells;
    public StatusViewModel Status { get; set; } = new();
}