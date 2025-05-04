using CodeBase.Core.Modules.Installer;
using UnityEngine;
using VContainer;

namespace Modules.Additional.SplashScreen.Scripts
{
    public class SplashScreenInstaller : SceneInstaller
    {
        [SerializeField] private SplashView splashView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterInstance(splashView).As<SplashView>();
            builder.Register<SplashScreenPresenter>(Lifetime.Singleton);
            builder.Register<SplashScreenModuleModel>(Lifetime.Singleton);
        }
    }
}