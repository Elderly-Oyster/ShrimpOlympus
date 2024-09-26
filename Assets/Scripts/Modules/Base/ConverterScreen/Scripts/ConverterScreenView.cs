using System.Globalization;
using Core.Scripts.MVP;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.Scripts.Base.ConverterScreen.Scripts
{
    public class ConverterScreenView : BaseScreenView
    {
        [SerializeField] private TMP_InputField sourceAmountInputField;
        [SerializeField] private TMP_InputField targetAmountInputField;
        [SerializeField] private TMP_Dropdown sourceCurrencyDropdown;
        [SerializeField] private TMP_Dropdown targetCurrencyDropdown;
        [SerializeField] private Scrollbar amountScrollBar;
        [SerializeField] private Button exitButton;

        
        protected override void Awake()
        {
            base.Awake();
            HideInstantly();
        }

        public void SetupEventListeners(
            ReactiveCommand<string> determineSourceCurrencyCommand,
            ReactiveCommand<string> determineTargetCurrencyCommand,
            ReactiveCommand<string> sourceAmountChangedCommand,
            ReactiveCommand<string> targetAmountChangedCommand,
            ReactiveCommand<float> handleAmountScrollBarChangedCommand,
            ReactiveCommand<Unit> backButtonCommand)
        {
            sourceAmountInputField.onValueChanged.AddListener(sourceAmountChangedCommand.Execute);
            targetAmountInputField.onValueChanged.AddListener(targetAmountChangedCommand.Execute);

            sourceCurrencyDropdown.onValueChanged.AddListener(index => determineSourceCurrencyCommand
                .Execute(sourceCurrencyDropdown.options[index].text));

            targetCurrencyDropdown.onValueChanged.AddListener(index => determineTargetCurrencyCommand
                .Execute(targetCurrencyDropdown.options[index].text));

            amountScrollBar.onValueChanged.AddListener(handleAmountScrollBarChangedCommand.Execute);

            exitButton.onClick.AddListener(() => backButtonCommand.Execute(default));
        }

        public float CurrentSourceAmount =>
            float.TryParse(sourceAmountInputField.text, out var r) ? r : 0f;

        public void UpdateSourceText(float amount) =>
            sourceAmountInputField.SetTextWithoutNotify(amount.ToString(CultureInfo.InvariantCulture));

        public void UpdateTargetText(float amount) =>
            targetAmountInputField.SetTextWithoutNotify(amount.ToString(CultureInfo.InvariantCulture));
    }
}
