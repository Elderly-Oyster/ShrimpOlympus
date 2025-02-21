using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.NewModuleScreen.Scripts
{
    public class NewModuleScreenInstaller : SceneInstaller
    {
        [SerializeField] private NewModuleScreenView newModuleScreenView;
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(newModuleScreenView).As<NewModuleScreenView>();
            builder.Register<NewModulePresenter>(Lifetime.Singleton);
            builder.Register<NewModuleScreenModel>(Lifetime.Singleton);
        }
    }
}