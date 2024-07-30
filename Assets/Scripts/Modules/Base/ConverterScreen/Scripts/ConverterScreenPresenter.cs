using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Modules.Additional.DynamicBackground;
using UnityEngine;
using VContainer;

namespace Modules.Base.ConverterScreen.Scripts
{
    public class ConverterScreenPresenter : IPresenter
    {
        [Inject] private readonly DynamicParticleController _dynamicParticleController;
        [Inject] private readonly ConverterScreenView _converterScreenView;
        private ConverterScreenModel _converterScreenModel;

        private readonly Dictionary<string, Currencies> _currencyToName = new()
        {
            { "EUR", Currencies.Eur },
            { "USD", Currencies.Usd },
            { "PLN", Currencies.Pln },
            { "PR", Currencies.Pr }
        };

        public void Initialize(ConverterScreenModel converterScreenModel)
        {
            _converterScreenModel = converterScreenModel;
            _converterScreenView.gameObject.SetActive(false);
            _converterScreenView.SetupEventListeners
            (
                DetermineSourceCurrency,
                DetermineTargetCurrency,
                OnSourceAmountChanged,
                OnTargetAmountChanged,
                HandleAmountScrollBarChanged,
                OnExitButtonClicked
            );
        }

        public async UniTask ShowView() => await _converterScreenView.Show();

        private void DetermineSourceCurrency(string name)
        {
            _converterScreenModel.SelectSourceCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.currentSourceAmount);
        }

        private void DetermineTargetCurrency(string name) 
        {
            _converterScreenModel.SelectTargetCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.currentSourceAmount);
        }

        private void OnSourceAmountChanged(string value)
        {
            if (float.TryParse(value, out var amount)) 
                CountTargetMoney(amount);
        }

        private void OnTargetAmountChanged(string value)
        {
            if (float.TryParse(value, out var amount)) 
                CountSourceMoney(amount);
        }

        private void CountTargetMoney(float count) =>
            _converterScreenView.UpdateTargetText(_converterScreenModel.ConvertSourceToTarget(count));

        private void CountSourceMoney(float count) =>
            _converterScreenView.UpdateSourceText(_converterScreenModel.ConvertTargetToSource(count));

        private void HandleAmountScrollBarChanged(float scrollValue)
        {
            _dynamicParticleController.parameter = scrollValue;
            var intValue = Mathf.RoundToInt(scrollValue * 200);
            _converterScreenView.UpdateSourceText(intValue);
            CountTargetMoney(intValue); 
        }

        private void OnExitButtonClicked() => _converterScreenModel.RunMainMenuModel();

        public void RemoveEventListeners() => _converterScreenView.RemoveEventListeners();
        public async UniTask HideScreenView() => await _converterScreenView.Hide();
    }
}
