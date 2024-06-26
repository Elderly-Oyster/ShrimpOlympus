using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using MVP.MVP_Root_Model.Scripts.Core;

namespace MVP.MVP_Root_Model.Scripts.Modules.ConverterScreen.Scripts
{
    public enum Currencies
    {
        Eur,
        Usd,
        Pln,
        Pr
    }
    public class ConverterScreenModel : IScreenModel
    {
        private readonly IRootController _rootController;
        private readonly ConverterScreenPresenter _converterScreenPresenter;
        private readonly UniTaskCompletionSource<Action> _completionSource;

        private Currencies _sourceCurrency;
        private Currencies _targetCurrency;

        private readonly Dictionary<Currencies, float> _currencyToEuroRate = new()
        {
            { Currencies.Eur, 1.0f },
            { Currencies.Usd, 1.1f },
            { Currencies.Pln, 4.6f },
            { Currencies.Pr, 0.05f }
        };

        public ConverterScreenModel(IRootController rootController, ConverterScreenPresenter converterScreenPresenter)
        {
            _completionSource = new UniTaskCompletionSource<Action>();
            _rootController = rootController;
            _converterScreenPresenter = converterScreenPresenter;
        }
        
        public async UniTask Run(object param)
        {
            _converterScreenPresenter.converterScreenModel = this;
            _converterScreenPresenter.Initialize();
            await _converterScreenPresenter.ShowView();
            var result = await _completionSource.Task;
            result.Invoke();
        }
        
        public void SelectSourceCurrency(Currencies currency) => _sourceCurrency = currency;
        public void SelectTargetCurrency(Currencies currency) => _targetCurrency = currency;

        public float ConvertSourceToTarget(float amount) =>
            ConvertCurrency(amount, _sourceCurrency, _targetCurrency);

        public float ConvertTargetToSource(float amount) =>
            ConvertCurrency(amount, _targetCurrency, _sourceCurrency);
        
        private float ConvertCurrency(float amount, Currencies from, Currencies to)
        {
            var amountInEuro = amount / _currencyToEuroRate[from];
            var convertedAmount = amountInEuro * _currencyToEuroRate[to];
            return convertedAmount;
        }

        public async UniTask Stop() => await _converterScreenPresenter.HideScreenView();
        public void Dispose() => _converterScreenPresenter.RemoveEventListeners();
    }
}