using Autofac;
using Viewer.Pages;

namespace Viewer;

public sealed class ViewerModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<GuiTimer>().AsImplementedInterfaces().InstancePerDependency();
    }
}