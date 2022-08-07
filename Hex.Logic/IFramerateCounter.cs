namespace Hex.Logic;

public interface IFramerateCounter : ITickable
{
    int Framerate { get; }
    void Start();
}