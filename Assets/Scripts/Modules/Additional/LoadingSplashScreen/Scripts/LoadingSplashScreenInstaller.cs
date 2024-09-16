using Core;
using Core.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Additional.LoadingSplashScreen.Scripts
{
    public class LoadingSplashScreenInstaller : SceneInstaller
    {
        [SerializeField] private LoadingSplashScreenView loadingSplashScreenView;
        [SerializeField] private ScreensCanvas screensCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(loadingSplashScreenView).As<LoadingSplashScreenView>();
            builder.Register<LoadingSplashScreenPresenter>(Lifetime.Singleton);
            builder.Register<LoadingSplashScreenModel>(Lifetime.Singleton);
        }
    }
}