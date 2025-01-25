using CodeBase.Core.Popups;
using CodeBase.Core.Systems.PopupHub;
using CodeBase.Implementation.Popups.FirstPopup;
using CodeBase.Implementation.Popups.SecondPopup;
using CodeBase.Implementation.Popups.ThirdPopup;
using CodeBase.Implementation.Systems.PopupHub;
using CodeBase.Implementation.UI;
using CodeBase.Services;
using CodeBase.Services.SceneInstallerService;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.Additional.PopupsManager.Scripts
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
            
            builder.Register<PopupHub>(Lifetime.Singleton)
                .AsImplementedInterfaces();
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