using System;

namespace Hex.Logic;

public interface ICompositeStartStoppable : IStartStoppable
{
    bool IsRunning { get; }
    void Add(IStartStoppable element);
    void Add(int updatesPerSecond, Action action);
}