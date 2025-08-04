using CodeBase.Core.UI;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Test.NewModuleScreen.Scripts
{
    public class NewModuleScreenInstaller : SceneInstaller
    {
        [SerializeField] private BaseScreenCanvas screenCanvas;
        [SerializeField] private Camera mainCamera;
        [SerializeField] private NewModuleScreenView newModuleScreenView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screenCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterComponent(newModuleScreenView).AsImplementedInterfaces().AsSelf();
            builder.Register<NewModuleStateController>(Lifetime.Singleton);
            builder.Register<NewModuleScreenModel>(Lifetime.Singleton);
        }
    }
}