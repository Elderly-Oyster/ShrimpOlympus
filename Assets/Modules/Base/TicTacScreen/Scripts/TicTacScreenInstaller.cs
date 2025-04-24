using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.TicTacScreen.Scripts
{
    public class TicTacScreenInstaller : SceneInstaller
    {
        [SerializeField] private TicTacView ticTacView;
        [SerializeField] private BaseScreenCanvas screensCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(ticTacView).As<TicTacView>();
            builder.Register<TicTacScreenPresenter>(Lifetime.Singleton);
            builder.Register<TicTacScreenModel>(Lifetime.Singleton);
        }
    }
}