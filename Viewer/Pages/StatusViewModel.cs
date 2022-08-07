using Hex.Logic;
using PropertyChanged.SourceGenerator;
using Stylet;

namespace Viewer.Pages;

public partial class StatusViewModel : IHandle<FramerateUpdate>
{
    [Notify] int _framerate;
    [Notify] HexCoordinate? _underMouse;
    [Notify] float _zoom;
    public StatusViewModel(IEventAggregator eventAggregator) => eventAggregator.Subscribe(this);

    public void Handle(FramerateUpdate message) => Framerate = message.Framerate;

    public string FramerateText => $"{Framerate} FPS";
    public string UnderMouseText => UnderMouse?.ToString() ?? "n/a";
}