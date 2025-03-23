using System.Threading.Tasks;
using CodeBase.Core.Modules;
using Cysharp.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameData;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;
using R3;
using Unit = R3.Unit;

namespace Modules.Base.DeliveryTycoon.Scripts.UpgradePopup
{
    public class UpgradePopupPresenter : IScreenPresenter
    {
        
        private readonly UpgradePopupView _upgradePopupView;
        private readonly GameScreenModel _gameScreenModel;
        private readonly TaskCompletionSource<bool> _screenCompletionSource;
        private readonly GameDataSystem _gameDataSystem;
        private readonly Mediator _mediator;
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
        
        public UpgradePopupPresenter(UpgradePopupView upgradePopupView, GameScreenModel gameScreenModel, GameDataSystem gameDataSystem, Mediator mediator)
        {
            _upgradePopupView = upgradePopupView;
            _gameScreenModel = gameScreenModel;
            _gameDataSystem = gameDataSystem;
            _mediator = mediator;
            _screenCompletionSource = new TaskCompletionSource<bool>();
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

        public async UniTask Execute() => await _screenCompletionSource.Task;


        public async UniTask Exit()
        {
            await _upgradePopupView.Hide();
            _screenCompletionSource.TrySetResult(true);
        }

        private void SubscribeToUIUpdates()
        {
            _onClosePopupCommand.Subscribe(_ => ClosePopup());
            _onBuyContainerCommand.Subscribe(_ => OnBuyContainerButtonClicked());
            _onPromoteCompanyCommand.Subscribe(_ => OnPromoteCompanyButtonClicked());
            _onAddCapacityCommand.Subscribe(_ => OnAddCapacityButtonClicked());
        }

        private void SubscribeToReactiveEvents()
        {
            _disposables.Add(_gameDataSystem.GameDataProperty.Subscribe(_ => CalculateUpgradeAllCosts(_)));
        }

        private void ClosePopup()
        {
            _gameScreenModel.ChangeState(GameScreenState.Game);
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