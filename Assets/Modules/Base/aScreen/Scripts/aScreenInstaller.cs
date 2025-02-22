using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.aScreen.Scripts
{
    public class aScreenInstaller : SceneInstaller
    {
        [SerializeField] private aScreenView aScreenView;
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(aScreenView).As<aScreenView>();
            builder.Register<aPresenter>(Lifetime.Singleton);
            builder.Register<aScreenModel>(Lifetime.Singleton);
        }
    }
}