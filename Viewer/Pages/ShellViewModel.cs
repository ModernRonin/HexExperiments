using System;
using System.Threading;
using System.Windows.Threading;
using Hex.Logic;
using PropertyChanged.SourceGenerator;
using Stylet;

namespace Viewer.Pages;

public sealed partial class ShellViewModel : Screen, IDisposable
{
    readonly HexConfigurationViewModel _configuration = new();
    readonly DispatcherTimer _timer = new(DispatcherPriority.Normal);
    readonly IWindowManager _windowManager;
    CancellationTokenSource _cancelSimulation;
    [Notify] Map _map;
    [Notify] string _toggleSimulationText;

    public ShellViewModel(IWindowManager windowManager)
    {
        _windowManager = windowManager;
        Map = Map.Create(_configuration.RingCount);
        _timer.Interval = TimeSpan.FromSeconds(1d / 20);
        _timer.Tick += OnTick;
        UpdateSimulationText();
    }

    public void Dispose() => _timer.Tick -= OnTick;

    public void ConfigureMap()
    {
        _configuration.RingCount = Map.Radius;
        if (_windowManager.ShowDialog(_configuration).GetValueOrDefault())
            Map = Map.Resize(_configuration.RingCount);
    }

    public void OnHexUnderPointerChanged(object _, EventArgs<HexCoordinate?> e) =>
        Status.UnderMouse = e.Value;

    public void OnZoomChanged(object _, EventArgs<float> e) => Status.Zoom = e.Value;

    public void ToggleSimulation()
    {
        if (_timer.IsEnabled)
        {
            _timer.Stop();
            _cancelSimulation.Cancel();
        }
        else
        {
            _cancelSimulation = new CancellationTokenSource();
            _map.Run(_cancelSimulation.Token);
            _timer.Start();
        }

        UpdateSimulationText();
    }

    public Cell[] Cells => Map.Cells;
    public StatusViewModel Status { get; set; } = new();

    void OnTick(object sender, EventArgs e)
    {
        NotifyOfPropertyChange(() => Cells);
    }

    void UpdateSimulationText() => ToggleSimulationText = _timer.IsEnabled ? "_Stop" : "_Start";
}