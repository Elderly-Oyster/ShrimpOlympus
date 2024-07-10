using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;
using MVP.MVP_Root_Model.Scripts.Modules.TicTacScreen.Scripts;
using VContainer;

namespace MVP.MVP_Root_Model.Scripts.Modules.ConverterScreen.Scripts
{
    public class ConverterScreenPresenter : IPresenter
    {
        [Inject] private readonly ConverterScreenView _converterScreenView;
        private ConverterScreenModel _converterScreenModel; //Без инжекта, т.к. появлялась Circle Dependency Exception

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
                CountTargetMoney,
                CountSourceMoney,
                OnExitButtonClicked
            );
        }

        public async UniTask ShowView() => await _converterScreenView.Show();

        // Меняет валюту-источник у модели и пересчитывает значение
        private void DetermineSourceCurrency(string name)
        {
            _converterScreenModel.SelectSourceCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.currentSourceAmount);
        }

        // Меняет таргет валюту у модели и пересчитывает значение
        private void DetermineTargetCurrency(string name) 
        {
            _converterScreenModel.SelectTargetCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.currentSourceAmount);
        }

        private void CountTargetMoney(float count) =>
            _converterScreenView.UpdateTargetText(_converterScreenModel.ConvertSourceToTarget(count));

        private void CountSourceMoney(float count) =>
            _converterScreenView.UpdateSourceText(_converterScreenModel.ConvertTargetToSource(count));

        private void OnExitButtonClicked()
        {
            _converterScreenModel.RunMainMenuModel();
        }

        public void RemoveEventListeners() => _converterScreenView.RemoveEventListeners();
        public async UniTask HideScreenView() => await _converterScreenView.Hide();
    }
}