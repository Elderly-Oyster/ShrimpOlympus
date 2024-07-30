using Core.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.Base.NewScreen.Scripts
{
    public class NewScreenView : UIView
    {
        [SerializeField] private Button mainMenuButton;
        
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