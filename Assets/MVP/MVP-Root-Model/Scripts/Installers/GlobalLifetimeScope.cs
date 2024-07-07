using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Services;
using MVP.MVP_Root_Model.Scripts.Startup;
using UnityEngine;
using VContainer;
using VContainer.Unity;

public class GlobalLifetimeScope : LifetimeScope
{
    [SerializeField] private ScreenTypeController screenTypeController;

    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ScreenTypeMapper>(Lifetime.Singleton);

        RegisterServices(builder);

        builder.RegisterInstance(screenTypeController).As<IRootController>();
    }

    private void RegisterServices(IContainerBuilder builder)
    {
        builder.Register<FirstLongInitializationService>(Lifetime.Singleton);
        builder.Register<SecondLongInitializationService>(Lifetime.Singleton);
        builder.Register<ThirdLongInitializationService>(Lifetime.Singleton);
    }
}
