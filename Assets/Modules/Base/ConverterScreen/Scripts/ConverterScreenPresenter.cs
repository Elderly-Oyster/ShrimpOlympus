using System.Collections.Generic;
using CodeBase.Core.Infrastructure;
using CodeBase.Core.Modules;
using CodeBase.Core.Modules.MVP;
using CodeBase.Core.Patterns.Architecture.MVP;
using Cysharp.Threading.Tasks;
using Modules.Additional.DynamicBackground.Scripts;
using R3;
using UnityEngine;

namespace Modules.Base.ConverterScreen.Scripts
{
    public class ConverterScreenPresenter : IModuleController
    {
        private readonly IScreenStateMachine _screenStateMachine;
        private readonly ConverterModuleModel _converterModuleModel;
        private readonly ConverterView _converterView;
        private readonly DynamicParticleController _dynamicParticleController;
        private readonly UniTaskCompletionSource<bool> _completionSource;
        
        private readonly ReactiveCommand<string> _determineSourceCurrencyCommand = new ReactiveCommand<string>();
        private readonly ReactiveCommand<string> _determineTargetCurrencyCommand = new ReactiveCommand<string>();
        private readonly ReactiveCommand<string> _sourceAmountChangedCommand = new ReactiveCommand<string>();
        private readonly ReactiveCommand<string> _targetAmountChangedCommand = new ReactiveCommand<string>();
        private readonly ReactiveCommand<float> _handleAmountScrollBarChangedCommand = new ReactiveCommand<float>();
        private readonly ReactiveCommand<Unit> _backButtonCommand = new ReactiveCommand<Unit>();


        public ConverterScreenPresenter(IScreenStateMachine screenStateMachine, ConverterModuleModel converterModuleModel, 
            ConverterView converterView, DynamicParticleController dynamicParticleController)
        {
            _screenStateMachine = screenStateMachine;
            _converterModuleModel = converterModuleModel;
            _converterView = converterView;
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
            _converterView.HideInstantly();
            _converterView.SetupEventListeners(
                _determineSourceCurrencyCommand,
                _determineTargetCurrencyCommand,
                _sourceAmountChangedCommand,
                _targetAmountChangedCommand,
                _handleAmountScrollBarChangedCommand,
                _backButtonCommand);
            await _converterView.Show();
        }

        public async UniTask Execute() => await _completionSource.Task;

        public async UniTask Exit() => await _converterView.Hide();

        private readonly Dictionary<string, Currencies> _currencyToName = new()
        {
            { "EUR", Currencies.Eur },
            { "USD", Currencies.Usd },
            { "PLN", Currencies.Pln },
            { "PR", Currencies.Pr }
        };

        private void DetermineSourceCurrency(string name)
        {
            _converterModuleModel.SelectSourceCurrency(_currencyToName[name]);
            CountTargetMoney(_converterView.CurrentSourceAmount);
        }

        private void DetermineTargetCurrency(string name) 
        {
            _converterModuleModel.SelectTargetCurrency(_currencyToName[name]);
            CountTargetMoney(_converterView.CurrentSourceAmount);
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
            _converterView.UpdateSourceText(_converterModuleModel.ConvertTargetToSource(count));

        private void HandleAmountScrollBarChanged(float scrollValue)
        {
            _dynamicParticleController.parameter = scrollValue;
            var intValue = Mathf.RoundToInt(scrollValue * 200);
            _converterView.UpdateSourceText(intValue);
            CountTargetMoney(intValue); 
        }
        
        private void CountTargetMoney(float count) =>
            _converterView.UpdateTargetText(_converterModuleModel.ConvertSourceToTarget(count));

        private void OnExitButtonClicked() => 
            RunNewScreen(ModulesMap.MainMenu);

        private void RunNewScreen(ModulesMap screen)
        {
            _completionSource.TrySetResult(true);
            _screenStateMachine.RunScreen(screen);
        }

        public void Dispose()
        {
            _converterView.Dispose();
            _converterModuleModel.Dispose();
        }
    }
}
