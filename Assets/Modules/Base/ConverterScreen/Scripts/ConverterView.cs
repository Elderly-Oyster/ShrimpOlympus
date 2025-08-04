using System.Globalization;
using CodeBase.Core.UI.Views;
using CodeBase.Services.Input;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace Modules.Base.ConverterScreen.Scripts
{
    public class ConverterView : BaseView
    {
        [SerializeField] private TMP_InputField sourceAmountInputField;
        [SerializeField] private TMP_InputField targetAmountInputField;
        [SerializeField] private TMP_Dropdown sourceCurrencyDropdown;
        [SerializeField] private TMP_Dropdown targetCurrencyDropdown;
        [SerializeField] private Scrollbar amountScrollBar;
        [SerializeField] private Button exitButton;

        private InputSystemService _inputSystemService;
        
        [Inject]
        private void Construct(InputSystemService inputSystemService)
        {
            _inputSystemService = inputSystemService;
        }
        
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
            
            exitButton.OnClickAsObservable()
                .Subscribe(_ => backButtonCommand.Execute(default))
                .AddTo(this);
            
            var escapePerformedObservable = 
                _inputSystemService.GetPerformedObservable(_inputSystemService.InputActions.UI.Cancel);
            
            escapePerformedObservable
                .Where(_ => IsInteractable)
                .Subscribe(_ => backButtonCommand.Execute(default))
                .AddTo(this);
        }

        public float CurrentSourceAmount =>
            float.TryParse(sourceAmountInputField.text, out var r) ? r : 0f;

        public void UpdateSourceText(float amount) =>
            sourceAmountInputField.SetTextWithoutNotify(amount.ToString(CultureInfo.InvariantCulture));

        public void UpdateTargetText(float amount) =>
            targetAmountInputField.SetTextWithoutNotify(amount.ToString(CultureInfo.InvariantCulture));
    }
}
