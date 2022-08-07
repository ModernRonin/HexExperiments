namespace Hex.Logic;

public interface IRunner
{
    void Start(params ITickable[] tickables);
    void Stop();
}