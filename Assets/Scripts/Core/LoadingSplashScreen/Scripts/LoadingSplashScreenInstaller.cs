using Core;
using Core.Popup.Base;
using Core.Popup.Types.FirstPopup.Scripts;
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
        [SerializeField] private FirstPopup firstPopupPrefab;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(loadingSplashScreenView).As<LoadingSplashScreenView>();
            builder.Register<LoadingSplashScreenPresenter>(Lifetime.Singleton);
            builder.Register<LoadingSplashScreenModel>(Lifetime.Singleton);
        }

        private void RegisterPopupFactories(IContainerBuilder builder)
        {
            builder.Register<BasePopupFactory<FirstPopup>>(Lifetime.Transient)
                .WithParameter(firstPopupPrefab)
                .AsImplementedInterfaces();
        }
    }
}