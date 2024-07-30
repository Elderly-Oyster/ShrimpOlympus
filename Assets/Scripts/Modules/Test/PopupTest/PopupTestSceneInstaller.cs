using Core;
using UnityEngine;
using VContainer;

namespace Modules.Test.PopupTest
{
    public class PopupTestSceneInstaller : SceneInstaller
    {
        [SerializeField] private PopupTestSceneView testView;
        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            Debug.Log("PopupTestSceneInstaller Init");
            builder.RegisterInstance(testView).As<PopupTestSceneView>();
        }

        public override void InjectSceneViews(IObjectResolver resolver) => 
            resolver.Inject(testView);
    }
}