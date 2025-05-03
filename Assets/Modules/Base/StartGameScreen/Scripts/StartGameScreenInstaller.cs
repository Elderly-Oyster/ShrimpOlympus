using CodeBase.Core.Modules.Installer;
using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.StartGameScreen.Scripts
{
    public class StartGameScreenInstaller : SceneInstaller
    {
        [SerializeField] private StartGameView startGameView;
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(startGameView).As<StartGameView>();
            builder.Register<StartGameScreenPresenter>(Lifetime.Singleton);
            builder.Register<StartGameScreenModel>(Lifetime.Singleton);
        }
    }
}