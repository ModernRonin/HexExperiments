using System.Diagnostics;
using Hex.Logic;
using Stylet;

namespace Viewer.Pages;

public class ShellViewModel : Screen
{
    readonly IWindowManager _windowManager;

    public ShellViewModel(IWindowManager windowManager)
    {
        _windowManager = windowManager;
        Map = Map.Create(Configuration.RingCount);
    }

    public void ConfigureMap()
    {
        Configuration.RingCount = Map.Radius;
        if (_windowManager.ShowDialog(Configuration).GetValueOrDefault())
        {
            Trace.WriteLine("resizing");
            Map.Resize(Configuration.RingCount);
        }
        else Trace.WriteLine("cancelled");
    }

    public HexConfigurationViewModel Configuration { get; set; } = new();
    public Map Map { get; }
    public StatusViewModel Status { get; set; } = new();
}