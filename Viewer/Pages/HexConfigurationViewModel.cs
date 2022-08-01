using Stylet;

namespace Viewer.Pages;

public class HexConfigurationViewModel : PropertyChangedBase
{
    int _ringCount = 5;

    public int RingCount
    {
        get => _ringCount;
        set => SetAndNotify(ref _ringCount, value);
    }
}