using UnityEngine;
using VContainer;

namespace Core.Popup.Scripts.PopupTest
{
    public class PopupTestSceneInstaller : SceneInstaller
    {
        [SerializeField] private PopupTestScreenView testView;
        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            Debug.Log("PopupTestSceneInstaller Init");
            builder.RegisterInstance(testView).As<PopupTestScreenView>();
        }
    }
}