using Core.Scripts.Services.SceneInstallerService;
using Core.Scripts.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Scripts.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenInstaller : SceneInstaller
    {
        [SerializeField] private MainMenuScreenView mainMenuScreenView;
        [SerializeField] private ScreensCanvas screensCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(mainMenuScreenView).As<MainMenuScreenView>();
            builder.Register<MainMenuScreenPresenter>(Lifetime.Singleton);
            builder.Register<MainMenuScreenModel>(Lifetime.Singleton);
        }
    }
}