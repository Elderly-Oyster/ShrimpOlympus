using MVP.MVP_Root_Model.Scripts.Core.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MVP.MVP_Root_Model.Scripts.Modules.MainMenuScreen.Scripts
{
    public class MainMenuScreenView : UIView
    {
        [SerializeField] private Button firstPopupButton;
        [SerializeField] private Button secondPopupButton;
        [SerializeField] private Button converterButton;
        [SerializeField] private Button ticTacButton;

        public void SetupEventListeners(
            UnityAction onConverterButtonClicked,
            UnityAction onTicTacButtonClicked,
            UnityAction onFirstPopupButtonClicked,
            UnityAction onSecondPopupButtonClicked)
        {
            converterButton.onClick.AddListener(onConverterButtonClicked);
            ticTacButton.onClick.AddListener(onTicTacButtonClicked);
            
            firstPopupButton.onClick.AddListener(onFirstPopupButtonClicked);
            secondPopupButton.onClick.AddListener(onSecondPopupButtonClicked);
        }

        public void RemoveEventListeners()
        {
            converterButton.onClick.RemoveAllListeners();
            ticTacButton.onClick.RemoveAllListeners();
        }
    }
}