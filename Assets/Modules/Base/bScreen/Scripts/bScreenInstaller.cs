using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.bScreen.Scripts
{
    public class bScreenInstaller : SceneInstaller
    {
        [SerializeField] private bScreenView bScreenView;
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(bScreenView).As<bScreenView>();
            builder.Register<bPresenter>(Lifetime.Singleton);
            builder.Register<bScreenModel>(Lifetime.Singleton);
        }
    }
}