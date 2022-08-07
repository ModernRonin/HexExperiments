using System;
using Hex.Logic;
using PropertyChanged.SourceGenerator;
using Stylet;

namespace Viewer.Pages;

public sealed partial class ShellViewModel : Screen
{
    const int UpdatesPerSecond = 20;
    readonly HexConfigurationViewModel _configuration = new();
    readonly ICompositeStartStoppable _startStoppable;
    readonly IWindowManager _windowManager;
    [Notify] ISimulation _simulation;
    [Notify] string _toggleSimulationText;

    public ShellViewModel(IWindowManager windowManager,
        StatusViewModel status,
        Func<int, ISimulation> simulationFactory,
        ICompositeStartStoppable startStoppable)
    {
        _windowManager = windowManager;
        _startStoppable = startStoppable;
        Status = status;
        Simulation = simulationFactory(_configuration.RingCount);
        _startStoppable.Add(Simulation);
        _startStoppable.Add(1, () => Status.Framerate = Simulation.Framerate);
        _startStoppable.Add(UpdatesPerSecond, () => NotifyOfPropertyChange(() => Cells));

        UpdateSimulationText();
    }

    public void ConfigureMap()
    {
        _configuration.RingCount = Simulation.Radius;
        if (_windowManager.ShowDialog(_configuration).GetValueOrDefault())
            Simulation = Simulation.Resize(_configuration.RingCount);
    }

    public void OnHexUnderPointerChanged(object _, EventArgs<HexCoordinate?> e) =>
        Status.UnderMouse = e.Value;

    public void OnZoomChanged(object _, EventArgs<float> e) => Status.Zoom = e.Value;

    public void ToggleSimulation()
    {
        if (_startStoppable.IsRunning) _startStoppable.Stop();
        else _startStoppable.Start();

        UpdateSimulationText();
    }

    public Cell[] Cells => Simulation.Map;

    public StatusViewModel Status { get; }

    void UpdateSimulationText() => ToggleSimulationText = _startStoppable.IsRunning ? "_Stop" : "_Start";
}