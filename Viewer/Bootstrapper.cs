using Autofac;
using Hex.Logic;
using Viewer.Pages;

namespace Viewer;

public class Bootstrapper : AutofacBootstrapper<ShellViewModel>
{
    protected override void ConfigureIoC(ContainerBuilder builder)
    {
        builder.RegisterModule<HexLogicModule>();
        builder.RegisterModule<ViewerModule>();
    }
}