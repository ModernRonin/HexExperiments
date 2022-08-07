using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;
using Stylet;

namespace Viewer;

public class AutofacBootstrapper<TRootViewModel> : BootstrapperBase where TRootViewModel : class
{
    IContainer _container;
    TRootViewModel _rootViewModel;

    protected override void ConfigureBootstrapper()
    {
        var builder = new ContainerBuilder();
        DefaultConfigureIoC(builder);
        ConfigureIoC(builder);
        _container = builder.Build();
    }

    public override object GetInstance(Type type) => _container.Resolve(type);

    protected override void Launch()
    {
        base.DisplayRootView(RootViewModel);
    }

    public override void Dispose()
    {
        ScreenExtensions.TryDispose(_rootViewModel);
        if (_container != null) _container.Dispose();

        base.Dispose();
    }

    protected virtual TRootViewModel RootViewModel =>
        _rootViewModel ??= (TRootViewModel)GetInstance(typeof(TRootViewModel));

    /// <summary>
    ///     Carries out default configuration of the IoC container. Override if you don't want to do this
    /// </summary>
    protected virtual void DefaultConfigureIoC(ContainerBuilder builder)
    {
        var viewManagerConfig = new ViewManagerConfig
        {
            ViewFactory = GetInstance,
            ViewAssemblies = new List<Assembly> { GetType().Assembly }
        };
        builder.RegisterInstance<IViewManager>(new ViewManager(viewManagerConfig));
        builder.RegisterType<MessageBoxView>();

        builder.RegisterInstance<IWindowManagerConfig>(this).ExternallyOwned();
        builder.RegisterType<WindowManager>().As<IWindowManager>().SingleInstance();
        builder.RegisterType<EventAggregator>().As<IEventAggregator>().SingleInstance();
        builder.RegisterType<MessageBoxViewModel>()
            .As<IMessageBoxViewModel>()
            .ExternallyOwned(); // Not singleton!

        // See https://github.com/canton7/Stylet/discussions/211
        builder.RegisterAssemblyTypes(GetType().Assembly)
            .Where(x => !x.Name.Contains("ProcessedByFody"))
            .ExternallyOwned();
    }

    /// <summary>
    ///     Override to add your own types to the IoC container.
    /// </summary>
    protected virtual void ConfigureIoC(ContainerBuilder builder) { }
}