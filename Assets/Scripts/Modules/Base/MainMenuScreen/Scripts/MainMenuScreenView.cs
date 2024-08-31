using CodeBase.Core.MVVM.View;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.Base.MainMenuScreen.Scripts
{
    public class MainMenuScreenView : BaseScreenView
    {
        [SerializeField] private Button firstPopupButton;
        [SerializeField] private Button secondPopupButton;
        [SerializeField] private Button converterButton;
        [SerializeField] private Button ticTacButton;
        
        protected override void Awake()
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