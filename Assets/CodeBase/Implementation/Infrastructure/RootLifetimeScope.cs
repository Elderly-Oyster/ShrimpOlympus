using CodeBase.Core.Systems.SaveSystem;
using CodeBase.Services;
using CodeBase.Services.LongInitializationServices;
using CodeBase.Services.SceneInstallerService;
using CodeBase.Systems;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Implementation.Infrastructure
{
    //RootLifeTimeScope where all the dependencies needed for the whole project are registered
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterServices(builder);

            RegisterSystems(builder);
                
            builder.Register<ScreenTypeMapper>(Lifetime.Singleton);

            builder.Register<ScreenStateMachine>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces();
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            var manager = new SerializableDataSystemsManager();
            manager.Initialize().Forget();
            builder.RegisterInstance(manager)
                .AsImplementedInterfaces()
                .AsSelf();
            builder.Register<GraphicsSettingsSystem>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
        }
        
        private void RegisterServices(IContainerBuilder builder)
        {
            RegisterLongInitializationService(builder);

            builder.Register<EventMediator>(Lifetime.Singleton)
                .AsImplementedInterfaces()
                .AsSelf();
            
            builder.Register<AudioListenerService>(Lifetime.Singleton)
                .AsSelf();
            
            builder.Register<EventSystemService>(Lifetime.Singleton)
                .As<IStartable>()
                .AsSelf();
            
            builder.Register<SceneService>(Lifetime.Singleton);
            builder.Register<SceneInstallerService>(Lifetime.Singleton);
        }

        private void RegisterLongInitializationService(IContainerBuilder builder)
        {
            builder.Register<FirstLongInitializationService>(Lifetime.Singleton);
            builder.Register<SecondLongInitializationService>(Lifetime.Singleton);
            builder.Register<ThirdLongInitializationService>(Lifetime.Singleton);
        }
    }
}