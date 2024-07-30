using Core;
using Core.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.NewScreen.Scripts
{
    public class NewInstaller : SceneInstaller
    {
        [SerializeField] private NewScreenView ticTacScreenView;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(ticTacScreenView).As<NewScreenView>();
            builder.Register<NewScreenPresenter>(Lifetime.Singleton);
            builder.Register<NewScreenModel>(Lifetime.Singleton);
        }
    }
}