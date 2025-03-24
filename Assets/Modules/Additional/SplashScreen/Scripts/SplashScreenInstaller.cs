using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace Modules.Additional.SplashScreen.Scripts
{
    public class SplashScreenInstaller : SceneInstaller
    {
        [SerializeField] private SplashScreenView splashScreenView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterInstance(splashScreenView);
            builder.Register<SplashScreenPresenter>(Lifetime.Singleton);
            builder.Register<SplashScreenModel>(Lifetime.Singleton);
        }
    }
}