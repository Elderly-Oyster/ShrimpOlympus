using Core;
using General.DynamicBackgroundScene;
using UnityEngine;
using VContainer;

namespace Modules.Additional.DynamicBackground
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