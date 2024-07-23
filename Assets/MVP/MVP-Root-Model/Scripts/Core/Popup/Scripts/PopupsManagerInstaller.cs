using MVP.MVP_Root_Model.Scripts.Core.Popup.Popups.FirstPopup.Scripts;
using MVP.MVP_Root_Model.Scripts.Core.Views.ProgressBars;
using MVP.MVP_Root_Model.Scripts.Services;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Core.Popup.Scripts
{
    public class PopupsManagerInstaller : SceneInstaller
    {
        [SerializeField] private PopupRootCanvas popupRootCanvas;
        [SerializeField] private FirstPopup firstPopupPrefab;
        [SerializeField] private PromotionPopup promotionPopup;
        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(popupRootCanvas);

            builder.Register<EventSystemService>(Lifetime.Singleton).As<IStartable>()
                .AsSelf();
            RegisterPopupFactories(builder);
            builder.Register<PopupHub>(Lifetime.Singleton);
        }
        
        private void RegisterPopupFactories(IContainerBuilder builder)
        {
            builder.Register<BasePopupFactory<FirstPopup>>(Lifetime.Transient)
                .WithParameter(firstPopupPrefab)
                .AsImplementedInterfaces(); 
            builder.Register<BasePopupFactory<PromotionPopup>>(Lifetime.Transient)
                .WithParameter(promotionPopup)
                .AsImplementedInterfaces(); 
        }
    }
}