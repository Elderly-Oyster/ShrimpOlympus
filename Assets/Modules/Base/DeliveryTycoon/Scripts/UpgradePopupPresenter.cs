using System.Threading.Tasks;
using CodeBase.Core.Modules;
using Cysharp.Threading.Tasks;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.Game;
using R3;
using UnityEngine;

namespace Modules.Base.DeliveryTycoon.Scripts
{
    public class UpgradePopupPresenter : IScreenPresenter
    {
        
        private UpgradePopupView _upgradePopupView;
        private GameScreenModel _gameScreenModel;
        private readonly TaskCompletionSource<bool> _screenCompletionSource;
        private GameDataSystem _gameDataSystem;
        private CompositeDisposable _disposables = new();
        
        private const int BaseUpgradesCount = 120;
        private const float UpgradeCostMultiplier = 1.5f;
        private int _upgradeCapacityCost;
        private int _upgradeMaxNumberOfOrdersCost;
        private int _hireEmployeeCost;
        private int _containerCost;
        
       
        public ReactiveCommand<Unit> OnBuyContainerButtonClickedCommand { get; private set; }
        
        public UpgradePopupPresenter(UpgradePopupView upgradePopupView, GameScreenModel gameScreenModel, GameDataSystem gameDataSystem)
        {
            _upgradePopupView = upgradePopupView;
            _gameScreenModel = gameScreenModel;
            _gameDataSystem = gameDataSystem;
            _screenCompletionSource = new TaskCompletionSource<bool>();
        }


        public UniTask Enter(object param)
        {
            _upgradePopupView.HideInstantly();
            SubscribeToReactiveEvents();
            _upgradePopupView.SetupEventListeners
            (
                ClosePopup,
                OnBuyContainerButtonClicked,
                OnPromoteCompanyButtonClicked,
                OnAddCapacityButtonClicked
                );
            CalculateUpgradeAllCosts(_gameDataSystem.GameDataProperty.CurrentValue);
            _upgradePopupView.UpdateAddCapacityText(_gameDataSystem.GameDataProperty.CurrentValue.capacity, 
                _upgradeCapacityCost);
            _upgradePopupView.UpdatePromoteCompanyText(_gameDataSystem.GameDataProperty.CurrentValue.maxNumberOfOrders,
                _upgradeMaxNumberOfOrdersCost);
            _upgradePopupView.UpdateHireEmployeeText(_hireEmployeeCost);
            SetInteractableButtons();
            return UniTask.CompletedTask;
        }

        public async UniTask Execute() => await _screenCompletionSource.Task;


        public async UniTask Exit()
        {
            await _upgradePopupView.Hide();
            _screenCompletionSource.TrySetResult(true);
        }

        private void SubscribeToReactiveEvents()
        {
            _disposables.Add(_gameDataSystem.GameDataProperty.Subscribe(CalculateUpgradeAllCosts));
        }

        private void ClosePopup()
        {
            Debug.Log("Close popup");
            _gameScreenModel.ChangeState(GameScreenState.UpgradePopup);
        }
        
        private void OnBuyContainerButtonClicked()
        {
            if (_gameDataSystem.GameDataProperty.CurrentValue.money > _containerCost)
            {
                _gameDataSystem.SetMoneyData(_gameDataSystem.GameDataProperty.Value.money - _containerCost);
                _gameDataSystem.InquireContainerInitialization();
            }
        }

        private void OnPromoteCompanyButtonClicked()
        {
            if (_gameDataSystem.GameDataProperty.CurrentValue.money > _upgradeMaxNumberOfOrdersCost)
            {
                _gameDataSystem.SetMoneyData(_gameDataSystem.GameDataProperty.Value.money - _upgradeMaxNumberOfOrdersCost);
                _gameDataSystem.SetMaxNumberOfOrdersData(++_gameDataSystem.GameDataProperty.Value.maxNumberOfOrders);
            }
        }

        private void OnAddCapacityButtonClicked()
        {
            if (_gameDataSystem.GameDataProperty.CurrentValue.money > _upgradeCapacityCost)
            {
                _gameDataSystem.SetMoneyData(_gameDataSystem.GameDataProperty.Value.money + _upgradeCapacityCost);
               _gameDataSystem.SetCapacityData(++_gameDataSystem.GameDataProperty.Value.capacity);
            }
        }
        
        private void SetInteractableButtons()
        {
            if (_gameDataSystem.GameDataProperty.CurrentValue.numberOfUnlockedUpgrades > 0)
            {
                for (int i = 0; i < _gameDataSystem.GameDataProperty.CurrentValue.numberOfUnlockedUpgrades; i++)
                    UnlockNewUpgradeOption();
            }
        }
        
        private void UnlockNewUpgradeOption()
        {
            var button = _upgradePopupView.Buttons[0];
            _upgradePopupView.InteractableButtons.Add(button);
            button.interactable = true;
            _upgradePopupView.Buttons.Remove(button);
        }
        
        private void CalculateUpgradeAllCosts(GameData gameData)
        {
            _upgradeCapacityCost = CalculateUpgradeCost(gameData.capacity);
            _upgradeMaxNumberOfOrdersCost = CalculateUpgradeCost(gameData.maxNumberOfOrders);
            _containerCost = 120;
            _hireEmployeeCost = 300;
        }
        
        private int CalculateUpgradeCost(int number)
        {
            return (int)(BaseUpgradesCount * (UpgradeCostMultiplier + number));
        }

        public void Dispose()
        {
            _disposables.Dispose();
        }
    }
}