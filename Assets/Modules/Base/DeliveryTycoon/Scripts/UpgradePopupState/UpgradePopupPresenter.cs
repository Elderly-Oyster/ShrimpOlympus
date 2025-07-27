using System.Threading.Tasks;
using CodeBase.Core.Patterns.Architecture.MVP;
using Cysharp.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;
using R3;
using UnityEngine;
using Unit = R3.Unit;

namespace Modules.Base.DeliveryTycoon.Scripts.UpgradePopupState
{
    public class UpgradePopupPresenter : IPresenter
    {
        
        private readonly UpgradePopupView _upgradePopupView;
        private readonly GameModel _gameModel;
        private TaskCompletionSource<bool> _screenCompletionSource;
        private readonly GameDataSystem _gameDataSystem;
        private readonly IMediator _mediator;
        private readonly CompositeDisposable _disposables = new();
        
        private const int BaseUpgradesCount = 120;
        private const float UpgradeCostMultiplier = 1.5f;
        private int _upgradeCapacityCost;
        private int _upgradeMaxNumberOfOrdersCost;
        private int _hireEmployeeCost;
        private int _containerCost;
        
        private readonly ReactiveCommand<Unit> _onClosePopupCommand = new();
        private readonly ReactiveCommand<Unit> _onBuyContainerCommand = new();
        private readonly ReactiveCommand<Unit> _onPromoteCompanyCommand = new();
        private readonly ReactiveCommand<Unit> _onAddCapacityCommand = new();
        
        public UpgradePopupPresenter(UpgradePopupView upgradePopupView, GameModel gameModel,
            GameDataSystem gameDataSystem, IMediator mediator)
        {
            _upgradePopupView = upgradePopupView;
            _gameModel = gameModel;
            _gameDataSystem = gameDataSystem;
            _mediator = mediator;
            
            
            _upgradePopupView.SetupEventListeners
            (
                _onClosePopupCommand,
                _onBuyContainerCommand,
                _onPromoteCompanyCommand,
                _onAddCapacityCommand
            );
            
            SubscribeToReactiveEvents();
            SubscribeToUIUpdates();
        }


        public async UniTask Enter(object param)
        {
            Debug.Log("Enter for upgrade popup");
            _screenCompletionSource = new TaskCompletionSource<bool>();
            _upgradePopupView.HideInstantly();
            SetInteractableButtons();
            CalculateUpgradeAllCosts(_gameDataSystem.GameDataProperty.CurrentValue);
            _upgradePopupView.UpdateAddCapacityText(_gameDataSystem.GameDataProperty.CurrentValue.capacity, 
                _upgradeCapacityCost);
            _upgradePopupView.UpdatePromoteCompanyText(_gameDataSystem.GameDataProperty.CurrentValue.maxNumberOfOrders,
                _upgradeMaxNumberOfOrdersCost);
            _upgradePopupView.UpdateHireEmployeeText(_hireEmployeeCost);
            await _upgradePopupView.Show();
        }

        
        public async UniTask Exit()
        {
            if (_upgradePopupView.isActiveAndEnabled)
            {
                await _upgradePopupView.Hide();
                _screenCompletionSource.TrySetResult(true);
            }
        }
        
        public async UniTask HideState() => await _upgradePopupView.Hide();
        
        public void HideStateInstantly() => _upgradePopupView.HideInstantly();

        private void SubscribeToUIUpdates()
        {
            _onClosePopupCommand.Subscribe(async _ => await ClosePopup());
            _onBuyContainerCommand.Subscribe(_ => OnBuyContainerButtonClicked());
            _onPromoteCompanyCommand.Subscribe(_ => OnPromoteCompanyButtonClicked());
            _onAddCapacityCommand.Subscribe(_ => OnAddCapacityButtonClicked());
        }

        private void SubscribeToReactiveEvents()
        {
            _disposables.Add(_gameDataSystem.GameDataProperty.Subscribe(CalculateUpgradeAllCosts));
        }

        private async UniTask ClosePopup()
        {
            await _gameModel.ChangeState(GameModuleStates.Game);
        }
        
        private async void OnBuyContainerButtonClicked()
        {
            var response = 
                await _mediator.Send(new CurrencyServiceOperations.CheckSufficientFunds(_containerCost));
            if (response)
                await _mediator.Send(new UpgradePopupOperations.ContainerBoughtCommand(_containerCost));
        }

        private async void OnPromoteCompanyButtonClicked()
        {
            var response =
                await _mediator.Send(
                    new CurrencyServiceOperations.CheckSufficientFunds(_upgradeMaxNumberOfOrdersCost));
            if (response)
            {
                await _mediator.Send(
                    new UpgradePopupOperations.CompanyPromotedCommand(_upgradeMaxNumberOfOrdersCost));
                _upgradePopupView.UpdatePromoteCompanyText(
                    _gameDataSystem.GameDataProperty.CurrentValue.maxNumberOfOrders,
                    _upgradeMaxNumberOfOrdersCost);
            }
        }

        private async void OnAddCapacityButtonClicked()
        {
            var response =
                await _mediator.Send(new CurrencyServiceOperations.CheckSufficientFunds(_upgradeCapacityCost));
            if (response) 
            {
                await _mediator.Send(new UpgradePopupOperations.CapacityIncreasedCommand(_upgradeCapacityCost));
                _upgradePopupView.UpdateAddCapacityText(_gameDataSystem.GameDataProperty.CurrentValue.capacity,
                    _upgradeCapacityCost);
            }
        }
        
        private void SetInteractableButtons()
        {
            if (_gameDataSystem.GameDataProperty.CurrentValue.numberOfUnlockedUpgrades > 0)
            {
                    UnlockNewUpgradeOption(_gameDataSystem.GameDataProperty.CurrentValue.numberOfUnlockedUpgrades);
            }
        }
        
        private void UnlockNewUpgradeOption(int availableUpgrades)
        {
            for (int i = 0; i < availableUpgrades; i++)
                _upgradePopupView.Buttons[i].interactable = true;
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