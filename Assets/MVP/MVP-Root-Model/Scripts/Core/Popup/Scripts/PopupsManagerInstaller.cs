using MVP.MVP_Root_Model.Scripts.Core.EventMediatorSystem;
using MVP.MVP_Root_Model.Scripts.Core.Popup.Popups.FirstPopup.Scripts;
using MVP.MVP_Root_Model.Scripts.Core.Views.ProgressBars;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MVP.MVP_Root_Model.Scripts.Core.Popup.Scripts
{
    public class PopupsManagerInstaller : SceneInstaller
    {
        [SerializeField] private PopupRootCanvas popupRootCanvas;
        [SerializeField] private FirstPopup firstPopupPrefab;
        [SerializeField] private SecondPopup secondPopup;
        [SerializeField] private ThirdPopup thirdPopup;
        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(popupRootCanvas);

            builder.Register<EventMediator>(Lifetime.Singleton);

            RegisterPopupFactories(builder);
            builder.Register<PopupHub>(Lifetime.Singleton);
        }
        
        private void RegisterPopupFactories(IContainerBuilder builder)
        {
            builder.Register<BasePopupFactory<FirstPopup>>(Lifetime.Transient)
                .WithParameter(firstPopupPrefab)
                .AsImplementedInterfaces(); 
            builder.Register<BasePopupFactory<SecondPopup>>(Lifetime.Transient)
                .WithParameter(secondPopup)
                .AsImplementedInterfaces();             
            builder.Register<BasePopupFactory<ThirdPopup>>(Lifetime.Transient)
                .WithParameter(thirdPopup)
                .AsImplementedInterfaces(); 
        }
    }
}