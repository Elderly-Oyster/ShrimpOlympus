using Core.Views.UIViews;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenView : FadeUIView
    {
        [SerializeField] private Button firstPopupButton;
        [SerializeField] private Button secondPopupButton;
        [SerializeField] private Button converterButton;
        [SerializeField] private Button ticTacButton;
        
        private new void Awake()
        {
            base.Awake();
            HideInstantly();
        }
        
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

        private void RemoveEventListeners()
        {
            converterButton.onClick.RemoveAllListeners();
            ticTacButton.onClick.RemoveAllListeners();
        }
        
        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }
    }
}