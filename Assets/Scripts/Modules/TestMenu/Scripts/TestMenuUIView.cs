using Core.UI.EnergyBar;
using Core.Views;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Modules.TestMenu.Scripts
{
    public class TestMenuUIView : UIView
    {
        [SerializeField] private EnergyBarView energyBarView;
        [SerializeField] private Button spendEnergyButton;
        [SerializeField] private Button getEnergyButton;
        
        // public float currentSourceAmount => 
        //     float.TryParse(sourceAmountInputField.text, out var r) ? r : 0f;
        //
        // public void UpdateSourceText(float amount) => 
        //     sourceAmountInputField.SetTextWithoutNotify(amount.ToString(CultureInfo.InvariantCulture));
        //
        // public void UpdateTargetText(float amount) => 
        //     targetAmountInputField.SetTextWithoutNotify(amount.ToString(CultureInfo.InvariantCulture));
        
        // public void SetupEventListeners(
        //     UnityAction<string> onSourceCurrencySelected,
        //     UnityAction<string> onTargetCurrencySelected,
        //     UnityAction<float> onSourceAmountChanged,
        //     UnityAction<float> onTargetAmountChanged)
        // {
        //     sourceAmountInputField.onValueChanged
        //         .AddListener(value => AddFloatInputFieldListener(value, onSourceAmountChanged));            
        //     targetAmountInputField.onValueChanged
        //         .AddListener(value => AddFloatInputFieldListener(value, onTargetAmountChanged));
        //     sourceCurrencyDropdown.onValueChanged
        //         .AddListener(index => onSourceCurrencySelected(sourceCurrencyDropdown.options[index].text));
        //     targetCurrencyDropdown.onValueChanged
        //         .AddListener(index => onTargetCurrencySelected(targetCurrencyDropdown.options[index].text));
        //     amountScrollBar.onValueChanged
        //         .AddListener(HandleAmountScrollBarChanged);
        // }   
        // public void RemoveEventListeners()
        // {
        //     sourceAmountInputField.onValueChanged.RemoveAllListeners();
        //     sourceCurrencyDropdown.onValueChanged.RemoveAllListeners();
        //     targetCurrencyDropdown.onValueChanged.RemoveAllListeners();
        //     amountScrollBar.onValueChanged.RemoveAllListeners();
        // }

        private static void AddFloatInputFieldListener(string inputText, UnityAction<float> onMoneyCountSelected)
        {
            if (!float.TryParse(inputText, out var fAmount)) return;
            onMoneyCountSelected(fAmount);
        }
        
        // private void HandleAmountScrollBarChanged(float scrollValue)
        // {
        //     var intValue = Mathf.RoundToInt(scrollValue * 200);
        //     sourceAmountInputField.text = intValue.ToString();
        // }
    }
}