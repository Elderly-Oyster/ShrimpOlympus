using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;

namespace Modules.Additional.SplashScreen.Scripts
{
    public class SplashScreenInstaller : SceneInstaller
    {
        [SerializeField] private SplashScreenView splashScreenView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterInstance(splashScreenView).As<SplashScreenView>();
            builder.Register<SplashScreenPresenter>(Lifetime.Singleton);
            builder.Register<SplashScreenModel>(Lifetime.Singleton);
        }
    }
}