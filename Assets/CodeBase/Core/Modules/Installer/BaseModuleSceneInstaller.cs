using CodeBase.Core.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Core.Modules.Installer
{
    public class BaseModuleSceneInstaller : SceneInstaller
    {
        [SerializeField] private BaseScreenCanvas screensCanvas;
        [SerializeField] private Camera mainCamera;
        
        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(screensCanvas);
            builder.RegisterInstance(mainCamera);
        }
    }
}