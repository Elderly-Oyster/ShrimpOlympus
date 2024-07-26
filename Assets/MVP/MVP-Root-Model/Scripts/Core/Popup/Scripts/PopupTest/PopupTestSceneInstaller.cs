using UnityEngine;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Core.Popup.Scripts
{
    public class PopupTestSceneInstaller : SceneInstaller
    {
        [SerializeField] private PopupTestScreenView testView;
        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            Debug.Log("OISDJFPOIJDSAFOP");
            builder.RegisterInstance(testView);
        }
    }
}