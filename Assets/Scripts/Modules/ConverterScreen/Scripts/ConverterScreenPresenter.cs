using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;

namespace Modules.ConverterScreen.Scripts
{
    public class ConverterScreenPresenter : IScreenPresenter
    {
        private readonly UniTaskCompletionSource<Action> _completionSource;
        private readonly IRootController _rootController; // Используем, для переключения по GUI модулям
        private readonly ConverterScreenView _converterScreenView;
        private readonly ConverterScreenModel _converterScreenModel;
        private readonly Dictionary<string, Currencies> _currencyToName = new()
        {
            {"EUR", Currencies.Eur},
            {"USD", Currencies.Usd},
            {"PLN", Currencies.Pln},
            {"PR", Currencies.Pr}
        };

        public ConverterScreenPresenter(IRootController rootController, ConverterScreenView converterScreenView, 
            ConverterScreenModel converterScreenModel)
        {
            _completionSource = new UniTaskCompletionSource<Action>();
            _rootController = rootController;
            _converterScreenView = converterScreenView;
            _converterScreenModel = converterScreenModel;
            _converterScreenView.gameObject.SetActive(false);
        }
        
        public async UniTask Run(object param)
        {
            _converterScreenView.SetupEventListeners
            (
                DetermineSourceCurrency, 
                DetermineTargetCurrency,
                CountTargetMoney, 
                CountSourceMoney
            );
            await _converterScreenView.Show();
            var result = await _completionSource.Task;
            result.Invoke();
        }

        private void DetermineSourceCurrency(string name)
        {
            _converterScreenModel.SelectSourceCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.currentSourceAmount);
        }

        private void DetermineTargetCurrency(string name)   // Меняет таргет валюту у модели
        {
            _converterScreenModel.SelectTargetCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.currentSourceAmount);
        }
        private void CountTargetMoney(float count) => 
            _converterScreenView.UpdateTargetText(_converterScreenModel.ConvertSourceToTarget(count));
        
        private void CountSourceMoney(float count) => 
            _converterScreenView.UpdateSourceText(_converterScreenModel.ConvertTargetToSource(count));

        public async UniTask Stop() => await _converterScreenView.Hide();

        public void Dispose()
        {
            _converterScreenView.RemoveEventListeners();
            _converterScreenView.Dispose();
        }
    }
}