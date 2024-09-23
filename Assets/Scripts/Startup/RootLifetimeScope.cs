using Core.EventMediatorSystem;
using Modules.Additional.LoadingSplashScreen.Scripts;
using Services;
using Services.LongInitializationServices;
using VContainer;
using VContainer.Unity;

namespace Startup
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            RegisterServices(builder);
            
            builder.Register<ScreenTypeMapper>(Lifetime.Singleton);

            builder.Register<ScreenStateMachine>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces();
        }

        private void RegisterServices(IContainerBuilder builder)
        {
            RegisterLongInitializationService(builder);
            
            builder.Register<EventMediator>(Lifetime.Singleton).AsSelf();

            builder.Register<AudioListenerService>(Lifetime.Singleton).As<IStartable>()
                .AsSelf();
            builder.Register<EventSystemService>(Lifetime.Singleton).As<IStartable>()
                .AsSelf();
            builder.Register<SplashScreenService>(Lifetime.Singleton)
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