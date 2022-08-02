using PropertyChanged.SourceGenerator;

namespace Viewer.Pages;

public partial class HexConfigurationViewModel
{
    [Notify] int _ringCount = 5;
}