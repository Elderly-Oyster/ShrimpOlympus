using MVP.MVP_Root_Model.Scripts.Core.EventMediatorSystem;
using MVP.MVP_Root_Model.Scripts.Core.Popup;
using MVP.MVP_Root_Model.Scripts.Core.Popup.Popups.FirstPopup.Scripts;
using MVP.MVP_Root_Model.Scripts.Core.Popup.Scripts;
using MVP.MVP_Root_Model.Scripts.Services;
using MVP.MVP_Root_Model.Scripts.Services.LongInitializationServices;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Startup
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterServices(builder);
            
            builder.Register<ScreenTypeMapper>(Lifetime.Singleton);

            builder.Register<ScreenController>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces();
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<AudioListenerService>(Lifetime.Singleton).As<IStartable>()
                .AsSelf();
            
            builder.Register<EventMediator>(Lifetime.Singleton);

            builder.Register<FirstLongInitializationService>(Lifetime.Singleton);
            builder.Register<SecondLongInitializationService>(Lifetime.Singleton);
            builder.Register<ThirdLongInitializationService>(Lifetime.Singleton);
            
            builder.Register<SceneService>(Lifetime.Singleton);
            builder.Register<SceneInstallerManager>(Lifetime.Singleton);
        }
    }
}