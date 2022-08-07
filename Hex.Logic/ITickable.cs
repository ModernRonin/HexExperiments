using System.Threading;

namespace Hex.Logic;

public interface ITickable
{
    void Tick(CancellationToken ct);
}