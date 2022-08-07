namespace Hex.Logic;

public interface IFramerateCounter
{
    int Framerate { get; }
    void Start();
    void Tick();
}