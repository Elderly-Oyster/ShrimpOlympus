using MVP.MVP_Root_Model.Scripts.Services;
using MVP.MVP_Root_Model.Scripts.Startup;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Installers
{
    public class RootLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ScreenTypeMapper>(Lifetime.Singleton);

            RegisterServices(builder);
                
            builder.Register<ScreenController>(Lifetime.Singleton)
                .AsSelf()
                .AsImplementedInterfaces();

        }

        private void RegisterServices(IContainerBuilder builder)
        {
            builder.Register<FirstLongInitializationService>(Lifetime.Singleton);
            builder.Register<SecondLongInitializationService>(Lifetime.Singleton);
            builder.Register<ThirdLongInitializationService>(Lifetime.Singleton);
        }
    }
}
