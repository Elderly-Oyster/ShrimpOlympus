using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using static Modules.MainMenu.Scripts.ConverterModel;

namespace Modules.MainMenu.Scripts
{
    public class ConverterPresenter : IScreenPresenter
    {
        private readonly UniTaskCompletionSource<Action> _completionSource;
        private readonly IRootController _rootController;   //TODO No more screens to switch in application
        private readonly ConverterUIView _converterUIView;
        private readonly ConverterModel _converterModel;
        private readonly Dictionary<string, Currencies> _currencyToName = new()
        {
            {"EUR", Currencies.Eur},
            {"USD", Currencies.Usd},
            {"PLN", Currencies.Pln},
            {"PR", Currencies.Pr}
        };

        public ConverterPresenter(IRootController rootController, ConverterUIView converterUIView, 
            ConverterModel converterModel)
        {
            _completionSource = new UniTaskCompletionSource<Action>();
            _rootController = rootController;
            _converterUIView = converterUIView;
            _converterModel = converterModel;
            _converterUIView.gameObject.SetActive(false);
        }
        
        public async UniTask Run(object param)
        {
            _converterUIView.SetupEventListeners
            (
                DetermineSourceCurrency, 
                DetermineTargetCurrency,
                CountTargetMoney, 
                CountSourceMoney
            );
            await _converterUIView.Show();
            var result = await _completionSource.Task;
            result.Invoke();
        }

        private void DetermineSourceCurrency(string name)
        {
            _converterModel.SelectSourceCurrency(_currencyToName[name]);
            CountTargetMoney(_converterUIView.currentSourceAmount);
        }

        private void DetermineTargetCurrency(string name)
        {
            _converterModel.SelectTargetCurrency(_currencyToName[name]);
            CountTargetMoney(_converterUIView.currentSourceAmount);
        }
        
        private void CountTargetMoney(float count) => 
            _converterUIView.UpdateTargetText(_converterModel.ConvertSourceToTarget(count));
        
        private void CountSourceMoney(float count) => 
            _converterUIView.UpdateSourceText(_converterModel.ConvertTargetToSource(count));

        public async UniTask Stop()
        {
            await _converterUIView.Hide();
        } 

        public void Dispose()
        {
            _converterUIView.RemoveEventListeners();
            _converterUIView.Dispose();
        }
    }
}