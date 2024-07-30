using Core;
using UnityEngine;
using VContainer;

namespace Modules.Test.PopupTest
{
    public class PopupTestSceneInstaller : SceneInstaller
    {
        [SerializeField] private PopupTestScreenView testView;
        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            Debug.Log("PopupTestSceneInstaller Init");
            builder.RegisterInstance(testView).As<PopupTestScreenView>();
        }

        public override void InjectSceneViews(IObjectResolver resolver) => 
            resolver.Inject(testView);
    }
}