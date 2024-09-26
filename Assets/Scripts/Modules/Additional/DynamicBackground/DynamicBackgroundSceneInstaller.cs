using Core.Scripts.Services.SceneInstallerService;
using UnityEngine;
using VContainer;

namespace Modules.Scripts.Additional.DynamicBackground
{
    public class DynamicBackgroundSceneInstaller : SceneInstaller
    {
        [SerializeField] private DynamicParticleController dynamicParticleController;
        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterInstance(dynamicParticleController);
        }
    }
}