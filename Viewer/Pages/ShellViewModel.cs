using Stylet;

namespace Viewer.Pages;

public class ShellViewModel : Screen
{
    public HexConfigurationViewModel Configuration { get; set; } = new();
}