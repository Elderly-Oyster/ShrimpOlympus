using MVP.MVP_Root_Model.Scripts.Core.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MVP.MVP_Root_Model.Scripts.Modules.NewScreen.Scripts
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