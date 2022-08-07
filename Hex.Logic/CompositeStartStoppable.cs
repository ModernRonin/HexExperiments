using System;
using System.Collections.Generic;

namespace Hex.Logic;

public sealed class CompositeStartStoppable : ICompositeStartStoppable
{
    readonly List<IStartStoppable> _elements = new();
    readonly Func<int, Action, PeriodicAction> _periodicActionFactory;

    public CompositeStartStoppable(Func<int, Action, PeriodicAction> periodicActionFactory) =>
        _periodicActionFactory = periodicActionFactory;

    public bool IsRunning { get; private set; }

    public void Add(IStartStoppable element) => _elements.Add(element);

    public void Start()

    {
        foreach (var part in _elements) part.Start();
        IsRunning = true;
    }

    public void Stop()
    {
        foreach (var part in _elements) part.Stop();
        IsRunning = false;
    }

    public void Add(int updatesPerSecond, Action action) =>
        _elements.Add(_periodicActionFactory(updatesPerSecond, action));
}