using PropertyChanged.SourceGenerator;
using Stylet;

namespace Viewer.Pages;

public partial class HexConfigurationViewModel : Screen
{
    [Notify] int _ringCount = 50;
    public void Cancel() => RequestClose();
    public void Confirm() => RequestClose(true);
}