using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Modules.ConverterScreen.Scripts
{
    public class ConverterScreenPresenter : IPresenter
    {
        [Inject] private readonly ConverterScreenView _converterScreenView;
        //TODO Не получилось заинжектить, т.к. появлялась Circle Dependency Exception
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

        // Меняет валюту-источник у модели и пересчитывает значение
        private void DetermineSourceCurrency(string name)
        {
            converterScreenModel.SelectSourceCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.currentSourceAmount);
        }

        // Меняет таргет валюту у модели и пересчитывает значение
        private void DetermineTargetCurrency(string name) 
        {
            converterScreenModel.SelectTargetCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.currentSourceAmount);
        }

        private void CountTargetMoney(float count) =>
            _converterScreenView.UpdateTargetText(converterScreenModel.ConvertSourceToTarget(count));

        private void CountSourceMoney(float count) =>
            _converterScreenView.UpdateSourceText(converterScreenModel.ConvertTargetToSource(count));

        public void RemoveEventListeners() => _converterScreenView.RemoveEventListeners();
        public async UniTask HideScreenView() => await _converterScreenView.Hide();
    }
}