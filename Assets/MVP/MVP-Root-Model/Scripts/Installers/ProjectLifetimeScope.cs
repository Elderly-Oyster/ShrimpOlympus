using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Services;
using MVP.MVP_Root_Model.Scripts.Startup;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Installers
{
    public class ProjectLifetimeScope : LifetimeScope
    {
        [SerializeField] private ScreenTypeController screenTypeController;

        protected override void Configure(IContainerBuilder builder)
        {
            Debug.Log("Initializing ProjectLifetimeScope");

            builder.Register<ScreenTypeMapper>(Lifetime.Singleton);

            RegisterServices(builder);

            builder.RegisterInstance(screenTypeController).As<IRootController>();

            Debug.Log("ProjectLifetimeScope configured successfully");
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<FirstLongInitializationService>(Lifetime.Singleton);
            builder.Register<SecondLongInitializationService>(Lifetime.Singleton);
            builder.Register<ThirdLongInitializationService>(Lifetime.Singleton);

            Debug.Log("Global services registered successfully");
        }
    }
}