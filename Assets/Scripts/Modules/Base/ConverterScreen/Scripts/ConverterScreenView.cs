using System.Globalization;
using Core.MVP;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static Core.UniRx.UniRxExtensions;

namespace Modules.Base.ConverterScreen.Scripts
{
    public class ConverterScreenView : BaseScreenView
    {
        [SerializeField] private TMP_InputField sourceAmountInputField;
        [SerializeField] private TMP_InputField targetAmountInputField;
        [SerializeField] private TMP_Dropdown sourceCurrencyDropdown;
        [SerializeField] private TMP_Dropdown targetCurrencyDropdown;
        [SerializeField] private Scrollbar amountScrollBar;
        [SerializeField] private Button exitButton;

        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        
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
            ReactiveCommand backButtonCommand)

        {
            sourceAmountInputField.OnValueChangedAsObservable()
                .Subscribe(name => sourceAmountChangedCommand.Execute(name))
                .AddTo(this);

            targetAmountInputField.OnValueChangedAsObservable()
                .Subscribe(name => targetAmountChangedCommand.Execute(name))
                .AddTo(this);

            sourceCurrencyDropdown.OnValueChangedAsObservable()
                .Subscribe(index => determineSourceCurrencyCommand.Execute(sourceCurrencyDropdown.options[index].text))
                .AddTo(this);

            targetCurrencyDropdown.OnValueChangedAsObservable()
                .Subscribe(index => determineTargetCurrencyCommand.Execute(targetCurrencyDropdown.options[index].text))
                .AddTo(this);

            amountScrollBar.OnValueChangedAsObservable()
                .Subscribe(value => handleAmountScrollBarChangedCommand.Execute(value))
                .AddTo(this);

            exitButton.OnClickAsObservable()
                .Subscribe(_ => backButtonCommand.Execute())
                .AddTo(this);
        }

        public float CurrentSourceAmount =>
            float.TryParse(sourceAmountInputField.text, out var r) ? r : 0f;

        public void UpdateSourceText(float amount) =>
            sourceAmountInputField.SetTextWithoutNotify(amount.ToString(CultureInfo.InvariantCulture));

        public void UpdateTargetText(float amount) =>
            targetAmountInputField.SetTextWithoutNotify(amount.ToString(CultureInfo.InvariantCulture));
        
        public override void Dispose()
        {
            RemoveEventListeners();
            base.Dispose();
        }

        private void RemoveEventListeners() => _disposables.Clear();
    }
}
