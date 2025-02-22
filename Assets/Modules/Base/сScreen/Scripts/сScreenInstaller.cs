using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.сScreen.Scripts
{
    public class сScreenInstaller : SceneInstaller
    {
        [SerializeField] private сScreenView сScreenView;
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(сScreenView).As<сScreenView>();
            builder.Register<сPresenter>(Lifetime.Singleton);
            builder.Register<сScreenModel>(Lifetime.Singleton);
        }
    }
}