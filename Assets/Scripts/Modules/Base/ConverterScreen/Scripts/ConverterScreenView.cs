using System.Globalization;
using CodeBase.Core.MVVM.View;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static Core.UniRxExtensions.UniRxExtensions;

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
            System.Action<string> onSourceCurrencySelected,
            System.Action<string> onTargetCurrencySelected,
            System.Action<string> onSourceAmountChanged,
            System.Action<string> onTargetAmountChanged,
            System.Action<float> onScrollBarValueChanged,
            System.Action onExitButtonClicked)
        {
            sourceAmountInputField.OnValueChangedAsObservable()
                .Subscribe(onSourceAmountChanged)
                .AddTo(_disposables);
        
            targetAmountInputField.OnValueChangedAsObservable()
                .Subscribe(onTargetAmountChanged)
                .AddTo(_disposables);
        
            sourceCurrencyDropdown.OnValueChangedAsObservable()
                .Subscribe(index => onSourceCurrencySelected(sourceCurrencyDropdown.options[index].text))
                .AddTo(_disposables);
        
            targetCurrencyDropdown.OnValueChangedAsObservable()
                .Subscribe(index => onTargetCurrencySelected(targetCurrencyDropdown.options[index].text))
                .AddTo(_disposables);
        
            amountScrollBar.OnValueChangedAsObservable()
                .Subscribe(onScrollBarValueChanged)
                .AddTo(_disposables);
        
            exitButton.OnClickAsObservable()
                .Subscribe(_ => onExitButtonClicked())
                .AddTo(_disposables);
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
