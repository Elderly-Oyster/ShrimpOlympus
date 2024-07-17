using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Startup;
using UnityEngine;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.General.DynamicBackgroundScene
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