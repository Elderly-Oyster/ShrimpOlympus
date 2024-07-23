using MVP.MVP_Root_Model.Scripts.Core;
using UnityEngine;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.General.PromotionAdditionalScene
{
    public class PromotionGUISceneInstaller : SceneInstaller
    {
        [SerializeField] private PromotionADView promotionADView;

        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterInstance(promotionADView);
        }
    }
}