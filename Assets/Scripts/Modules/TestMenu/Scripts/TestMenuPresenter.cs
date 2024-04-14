using System;
using System.Collections.Generic;
using Core;
using Cysharp.Threading.Tasks;
using Modules.TestMenu.Scripts;

namespace Modules.ConverterScreen.Scripts
{
    public class TestMenuPresenter : IScreenPresenter
    {
        private readonly UniTaskCompletionSource<Action> _completionSource;
        private readonly IRootController _rootController; // Используем, для переключения по GUI модулям
        private readonly TestMenuUIView _testMenuUIView;
        private readonly TestMenuModel _testMenuModel;
        private readonly Dictionary<string, Currencies> _currencyToName = new()
        {
            {"EUR", Currencies.Eur},
            {"USD", Currencies.Usd},
            {"PLN", Currencies.Pln},
            {"PR", Currencies.Pr}
        };

        public TestMenuPresenter(IRootController rootController, TestMenuUIView testMenuUIView, 
            TestMenuModel testMenuModel)
        {
            _completionSource = new UniTaskCompletionSource<Action>();
            _rootController = rootController;
            _testMenuUIView = testMenuUIView;
            _testMenuModel = testMenuModel;
            _testMenuUIView.gameObject.SetActive(false);
        }
        
        public async UniTask Run(object param)
        {
            _testMenuUIView.SetupEventListeners
            (
                DetermineSourceCurrency, 
                DetermineTargetCurrency,
                CountTargetMoney, 
                CountSourceMoney
            );
            await _testMenuUIView.Show();
            var result = await _completionSource.Task;
            result.Invoke();
        }

        private void DetermineSourceCurrency(string name)
        {
            _testMenuModel.SelectSourceCurrency(_currencyToName[name]);
            CountTargetMoney(_testMenuUIView.currentSourceAmount);
        }

        private void DetermineTargetCurrency(string name)
        {
            _testMenuModel.SelectTargetCurrency(_currencyToName[name]);
            CountTargetMoney(_testMenuUIView.currentSourceAmount);
        }
        
        private void CountTargetMoney(float count) => 
            _testMenuUIView.UpdateTargetText(_testMenuModel.ConvertSourceToTarget(count));
        
        private void CountSourceMoney(float count) => 
            _testMenuUIView.UpdateSourceText(_testMenuModel.ConvertTargetToSource(count));

        public async UniTask Stop() => await _testMenuUIView.Hide();

        public void Dispose()
        {
            _testMenuUIView.RemoveEventListeners();
            _testMenuUIView.Dispose();
        }
    }
}