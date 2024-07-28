using Scripts.Core;
using Scripts.Core.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Scripts.Modules.TicTacScreen.Scripts
{
    public class TicTacInstaller : SceneInstaller
    {
        [SerializeField] private TicTacScreenView ticTacScreenView;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(ticTacScreenView).As<TicTacScreenView>();
            builder.Register<TicTacScreenPresenter>(Lifetime.Singleton);
            builder.Register<TicTacScreenModel>(Lifetime.Singleton);
        }
    }
}