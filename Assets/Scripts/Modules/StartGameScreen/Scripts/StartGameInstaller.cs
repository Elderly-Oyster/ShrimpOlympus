using Core;
using Core.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.StartGameScreen.Scripts
{
    public class StartGameInstaller : SceneInstaller
    {
        [SerializeField] private StartGameScreenView startGameScreenView;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(startGameScreenView).As<StartGameScreenView>();
            builder.Register<StartGameScreenPresenter>(Lifetime.Singleton);
            builder.Register<StartGameScreenModel>(Lifetime.Singleton);
        }
    }
}