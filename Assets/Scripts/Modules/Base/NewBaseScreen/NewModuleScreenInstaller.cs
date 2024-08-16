using Core;
using Core.Views;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Base.NewBaseScreen
{
    public class NewModuleScreenInstaller : SceneInstaller
    {
        [SerializeField] private NewModuleScreenView newModuleScreenView;
        [SerializeField] private RootCanvas rootCanvas;
        [SerializeField] private Camera mainCamera;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(rootCanvas);
            builder.RegisterInstance(mainCamera);

            builder.RegisterInstance(newModuleScreenView).As<NewModuleScreenView>();
            builder.Register<NewModuleScreenViewModel>(Lifetime.Singleton);
            builder.Register<NewModuleScreenModel>(Lifetime.Singleton);
        }
    }
}