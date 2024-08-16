using Core.Views;
using Core.Views.UIViews;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.Base.NewScreen.Scripts
{
    public class NewScreenView : FadeUIView
    {
        [SerializeField] private Button mainMenuButton;
        
        private new void Awake()
        {
            base.Awake();
            HideInstantly();
        }
        
        public void SetupEventListeners(UnityAction onMainMenuButtonClicked)
        {
            mainMenuButton.onClick.AddListener(onMainMenuButtonClicked);
        }   
        public void RemoveEventListeners()
        {
            mainMenuButton.onClick.RemoveAllListeners();
        }
    }
}