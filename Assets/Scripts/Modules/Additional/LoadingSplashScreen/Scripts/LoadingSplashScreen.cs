using Core;
using Core.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.StartGameScreen.Scripts
{
    public class LoadingSplashScreenInstaller : SceneInstaller
    {
        [SerializeField] private LoadingSplashScreenView _loadingSplashScreenView;
        [SerializeField] private ScreensCanvas screensCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(_loadingSplashScreenView).As<LoadingSplashScreenView>();
            builder.Register<LoadingSplashScreenPresenter>(Lifetime.Singleton);
            builder.Register<LoadingSplashScreenModel>(Lifetime.Singleton);
        }
    }
}