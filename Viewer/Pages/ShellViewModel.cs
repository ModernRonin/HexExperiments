﻿using System;
using System.Threading;
using System.Windows.Threading;
using Hex.Logic;
using PropertyChanged.SourceGenerator;
using Stylet;

namespace Viewer.Pages;

public sealed partial class ShellViewModel : Screen, IDisposable
{
    const int UpdatesPerSecond = 20;
    readonly HexConfigurationViewModel _configuration = new();
    readonly DispatcherTimer _framerateTimer = new(DispatcherPriority.Background);
    readonly DispatcherTimer _renderTimer = new(DispatcherPriority.Normal);
    readonly IWindowManager _windowManager;
    CancellationTokenSource _cancelSimulation;
    [Notify] Simulation _simulation;
    [Notify] string _toggleSimulationText;

    public ShellViewModel(IWindowManager windowManager,
        StatusViewModel status)
    {
        _windowManager = windowManager;
        Status = status;
        Simulation = Simulation.Create(_configuration.RingCount);
        _renderTimer.Interval = TimeSpan.FromSeconds(1d / UpdatesPerSecond);
        _renderTimer.Tick += OnRenderTick;
        _framerateTimer.Interval = TimeSpan.FromSeconds(1);
        _framerateTimer.Tick += OnFramerateTick;

        UpdateSimulationText();
    }

    public void Dispose()
    {
        _renderTimer.Tick -= OnRenderTick;
        _framerateTimer.Tick -= OnFramerateTick;
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
        if (_renderTimer.IsEnabled)
        {
            _renderTimer.Stop();
            _framerateTimer.Stop();
            _cancelSimulation.Cancel();
        }
        else
        {
            _cancelSimulation = new CancellationTokenSource();
            _simulation.Run(_cancelSimulation.Token);
            _renderTimer.Start();
            _framerateTimer.Start();
        }

        UpdateSimulationText();
    }

    public Cell[] Cells => Simulation.Map;

    public StatusViewModel Status { get; }

    void OnFramerateTick(object sender, EventArgs e) => Status.Framerate = Simulation.Framerate;

    void OnRenderTick(object sender, EventArgs e) => NotifyOfPropertyChange(() => Cells);

    void UpdateSimulationText() => ToggleSimulationText = _renderTimer.IsEnabled ? "_Stop" : "_Start";
}