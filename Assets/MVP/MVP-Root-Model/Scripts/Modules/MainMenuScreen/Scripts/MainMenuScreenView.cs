using MVP.MVP_Root_Model.Scripts.Core.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MVP.MVP_Root_Model.Scripts.Modules.MainMenuScreen.Scripts
{
    public class MainMenuScreenView : UIView
    {
        [SerializeField] private Button converterButton;
        [SerializeField] private Button featureButton;

        public void SetupEventListeners(
            UnityAction onConverterButtonClicked,
            UnityAction onFeatureButtonSelected)
        {
            converterButton.onClick.AddListener(() => OnConverterButtonClicked(onConverterButtonClicked));
            featureButton.onClick.AddListener(() => OnFeatureButtonClicked(onFeatureButtonSelected));
        }   

        public void RemoveEventListeners()
        {
            converterButton.onClick.RemoveAllListeners();
            featureButton.onClick.RemoveAllListeners();
        }

        private void OnConverterButtonClicked(UnityAction onConverterButtonClicked)
        {
            onConverterButtonClicked();
        }        
        
        private void OnFeatureButtonClicked(UnityAction onFeatureButtonClicked)
        {
            onFeatureButtonClicked();
        }
    }
}