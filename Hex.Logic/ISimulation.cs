namespace Hex.Logic;

public interface ISimulation : ITickable, IStartStoppable
{
    int Framerate { get; }
    Cell[] Map { get; }
    int Radius { get; }
    Simulation Resize(int newRadius);
}