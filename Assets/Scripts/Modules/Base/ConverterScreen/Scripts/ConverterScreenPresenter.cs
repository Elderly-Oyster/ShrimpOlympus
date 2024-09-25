using System.Collections.Generic;
using Core;
using Core.MVP;
using Cysharp.Threading.Tasks;
using Implementation;
using Modules.Additional.DynamicBackground;
using R3;
using UnityEngine;

namespace Modules.Base.ConverterScreen.Scripts
{
    public class ConverterScreenPresenter : IScreenPresenter
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly ConverterScreenModel _converterScreenModel;
        private readonly ConverterScreenView _converterScreenView;
        private readonly DynamicParticleController _dynamicParticleController;
        private readonly UniTaskCompletionSource<bool> _completionSource;
        
        private readonly ReactiveCommand<string> _determineSourceCurrencyCommand = new ReactiveCommand<string>();
        private readonly ReactiveCommand<string> _determineTargetCurrencyCommand = new ReactiveCommand<string>();
        private readonly ReactiveCommand<string> _sourceAmountChangedCommand = new ReactiveCommand<string>();
        private readonly ReactiveCommand<string> _targetAmountChangedCommand = new ReactiveCommand<string>();
        private readonly ReactiveCommand<float> _handleAmountScrollBarChangedCommand = new ReactiveCommand<float>();
        private readonly ReactiveCommand<Unit> _backButtonCommand = new ReactiveCommand<Unit>();


        public ConverterScreenPresenter(IScreenStateMachine screenStateMachine, ConverterScreenModel converterScreenModel, 
            ConverterScreenView converterScreenView, DynamicParticleController dynamicParticleController)
        {
            _screenStateMachine = screenStateMachine;
            _converterScreenModel = converterScreenModel;
            _converterScreenView = converterScreenView;
            _dynamicParticleController = dynamicParticleController;
            
            _completionSource = new UniTaskCompletionSource<bool>();

            SubscribeToUIUpdates();
        }

        private void SubscribeToUIUpdates()
        {
            _backButtonCommand.Subscribe(_ => OnExitButtonClicked());
            _determineSourceCurrencyCommand.Subscribe(DetermineSourceCurrency);
            _determineTargetCurrencyCommand.Subscribe(DetermineTargetCurrency);
            _sourceAmountChangedCommand.Subscribe(OnSourceAmountChanged);
            _targetAmountChangedCommand.Subscribe(OnTargetAmountChanged);
            _handleAmountScrollBarChangedCommand.Subscribe(HandleAmountScrollBarChanged);
        }

        public async UniTask Enter(object param)
        {
            _converterScreenView.HideInstantly();
            _converterScreenView.SetupEventListeners(
                _determineSourceCurrencyCommand,
                _determineTargetCurrencyCommand,
                _sourceAmountChangedCommand,
                _targetAmountChangedCommand,
                _handleAmountScrollBarChangedCommand,
                _backButtonCommand);
            await _converterScreenView.Show();
        }

        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit() => await _converterScreenView.Hide();

        private readonly Dictionary<string, Currencies> _currencyToName = new()
        {
            { "EUR", Currencies.Eur },
            { "USD", Currencies.Usd },
            { "PLN", Currencies.Pln },
            { "PR", Currencies.Pr }
        };

        private void DetermineSourceCurrency(string name)
        {
            _converterScreenModel.SelectSourceCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.CurrentSourceAmount);
        }

        private void DetermineTargetCurrency(string name) 
        {
            _converterScreenModel.SelectTargetCurrency(_currencyToName[name]);
            CountTargetMoney(_converterScreenView.CurrentSourceAmount);
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

        private void CountSourceMoney(float count) =>
            _converterScreenView.UpdateSourceText(_converterScreenModel.ConvertTargetToSource(count));

        private void HandleAmountScrollBarChanged(float scrollValue)
        {
            _dynamicParticleController.parameter = scrollValue;
            var intValue = Mathf.RoundToInt(scrollValue * 200);
            _converterScreenView.UpdateSourceText(intValue);
            CountTargetMoney(intValue); 
        }
        
        private void CountTargetMoney(float count) =>
            _converterScreenView.UpdateTargetText(_converterScreenModel.ConvertSourceToTarget(count));

        private void OnExitButtonClicked() => 
            RunNewScreen(ScreenPresenterMap.MainMenu);

        private void RunNewScreen(ScreenPresenterMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }

        public void Dispose()
        {
            _converterScreenView.Dispose();
            _converterScreenModel.Dispose();
        }
    }
}
