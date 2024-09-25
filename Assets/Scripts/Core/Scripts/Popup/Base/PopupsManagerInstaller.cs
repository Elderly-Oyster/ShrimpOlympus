using Core.Scripts.EventMediatorSystem;
using Core.Scripts.Popup.Types.FirstPopup.Scripts;
using Core.Scripts.Popup.Types.SecondPopup.Scripts;
using Core.Scripts.Popup.Types.ThirdPopup.Scripts;
using Core.Scripts.Services.SceneInstallerService;
using Core.Scripts.Views.ProgressBars;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Core.Scripts.Popup.Base
{
    public class PopupsManagerInstaller : SceneInstaller
    {
        [SerializeField] private PopupCanvas popupCanvas;
        [SerializeField] private FirstPopup firstPopupPrefab;
        [SerializeField] private SecondPopup secondPopup;
        [SerializeField] private ThirdPopup thirdPopup;
        public override void RegisterSceneDependencies(IContainerBuilder builder)
        {
            builder.RegisterComponent(popupCanvas);

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