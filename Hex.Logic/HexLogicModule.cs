using Autofac;

namespace Hex.Logic;

public sealed class HexLogicModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<DefaultClock>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<DeterministicRandomity>().AsImplementedInterfaces().SingleInstance();
        builder.RegisterType<Simulation>().AsImplementedInterfaces().SingleInstance();

        builder.RegisterType<FramerateCounter>().AsImplementedInterfaces().InstancePerDependency();
        builder.RegisterType<Runner>().AsImplementedInterfaces().InstancePerDependency();
    }
}