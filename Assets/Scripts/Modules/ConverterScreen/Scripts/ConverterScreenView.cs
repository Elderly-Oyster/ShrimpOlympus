using System.Globalization;
using Scripts.Core.Views;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Scripts.Modules.ConverterScreen.Scripts
{
    public class ConverterScreenView : UIView
    {
        [SerializeField] private TMP_InputField sourceAmountInputField;
        [SerializeField] private TMP_InputField targetAmountInputField;
        [SerializeField] private TMP_Dropdown sourceCurrencyDropdown;
        [SerializeField] private TMP_Dropdown targetCurrencyDropdown;
        [SerializeField] private Scrollbar amountScrollBar;
        [SerializeField] private Button exitButton;

        public float currentSourceAmount =>
            float.TryParse(sourceAmountInputField.text, out var r) ? r : 0f;

        public void UpdateSourceText(float amount) =>
            sourceAmountInputField.SetTextWithoutNotify(amount.ToString(CultureInfo.InvariantCulture));

        public void UpdateTargetText(float amount) =>
            targetAmountInputField.SetTextWithoutNotify(amount.ToString(CultureInfo.InvariantCulture));

        public void SetupEventListeners(
            UnityAction<string> onSourceCurrencySelected,
            UnityAction<string> onTargetCurrencySelected,
            UnityAction<string> onSourceAmountChanged,
            UnityAction<string> onTargetAmountChanged,
            UnityAction<float> onScrollBarValueChanged,
            UnityAction onExitButtonClicked)
        {
            sourceAmountInputField.onValueChanged
                .AddListener(onSourceAmountChanged);            
            targetAmountInputField.onValueChanged
                .AddListener(onTargetAmountChanged);
            sourceCurrencyDropdown.onValueChanged
                .AddListener(index => onSourceCurrencySelected(sourceCurrencyDropdown.options[index].text));
            targetCurrencyDropdown.onValueChanged
                .AddListener(index => onTargetCurrencySelected(targetCurrencyDropdown.options[index].text));
            amountScrollBar.onValueChanged
                .AddListener(onScrollBarValueChanged);
            exitButton.onClick.AddListener(onExitButtonClicked);
        }

        public void RemoveEventListeners()
        {
            sourceAmountInputField.onValueChanged.RemoveAllListeners();
            sourceCurrencyDropdown.onValueChanged.RemoveAllListeners();
            targetCurrencyDropdown.onValueChanged.RemoveAllListeners();
            amountScrollBar.onValueChanged.RemoveAllListeners();
        }
    }
}
