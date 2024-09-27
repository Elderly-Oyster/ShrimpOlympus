using Core.Scripts.Services.SceneInstallerService;
using Core.Scripts.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.TicTacScreen.Scripts
{
    public class TicTacScreenInstaller : SceneInstaller
    {
        [SerializeField] private TicTacScreenView ticTacScreenView;
        [SerializeField] private ScreensCanvas screensCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(ticTacScreenView).As<TicTacScreenView>();
            builder.Register<TicTacScreenPresenter>(Lifetime.Singleton);
            builder.Register<TicTacScreenModel>(Lifetime.Singleton);
        }
    }
}