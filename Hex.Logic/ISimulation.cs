using System.Threading;
using System.Threading.Tasks;

namespace Hex.Logic;

public interface ISimulation
{
    int Framerate { get; }
    Cell[] Map { get; }
    int Radius { get; }
    Simulation Resize(int newRadius);
    Task Run(CancellationToken ct);
}