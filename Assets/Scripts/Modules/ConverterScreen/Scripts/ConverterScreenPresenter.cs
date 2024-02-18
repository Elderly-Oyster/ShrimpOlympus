using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using VContainer;

namespace Modules.ConverterScreen.Scripts
{
    public class ConverterScreenPresenter : IPresenter
    {
        
        [Inject] private readonly ConverterScreenView _converterScreenView;
        public ConverterScreenModel converterScreenModel { get; set; }

        private readonly Dictionary<string, Currencies> _currencyToName = new()
        {
            { "EUR", Currencies.Eur },
            { "USD", Currencies.Usd },
            { "PLN", Currencies.Pln },
            { "PR", Currencies.Pr }
        };

        public void Initialize()
        {
            _converterScreenView.gameObject.SetActive(false);
            _converterScreenView.SetupEventListeners
            (
                DetermineSourceCurrency,
                DetermineTargetCurrency,
                CountTargetMoney,
                CountSourceMoney
            );
        }

        public async UniTask ShowView() => await _converterScreenView.Show();

        private void DetermineSourceCurrency(string name)
        {
            converterScreenModel.SelectSourceCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.currentSourceAmount);
        }

        private void DetermineTargetCurrency(string name) // Меняет таргет валюту у модели
        {
            converterScreenModel.SelectTargetCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.currentSourceAmount);
        }

        private void CountTargetMoney(float count) =>
            _converterScreenView.UpdateTargetText(converterScreenModel.ConvertSourceToTarget(count));

        private void CountSourceMoney(float count) =>
            _converterScreenView.UpdateSourceText(converterScreenModel.ConvertTargetToSource(count));

        private void RemoveEventListeners() => _converterScreenView.RemoveEventListeners();
        
        public async UniTask HideScreenView()
        {
            await _converterScreenView.Hide();
            RemoveEventListeners();
        }

    }
}