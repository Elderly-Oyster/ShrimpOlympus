using Core;
using UnityEngine;
using VContainer;

namespace General.DynamicBackgroundScene
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