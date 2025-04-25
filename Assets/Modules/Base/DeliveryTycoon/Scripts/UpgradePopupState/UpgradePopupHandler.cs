using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.Player;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Managers.ContainerManagerLogic;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;

namespace Modules.Base.DeliveryTycoon.Scripts.UpgradePopup
{
    public class UpgradePopupHandler : 
        IRequestHandler<UpgradePopupOperations.ContainerBoughtCommand>,
        IRequestHandler<UpgradePopupOperations.CompanyPromotedCommand>,
        IRequestHandler<UpgradePopupOperations.CapacityIncreasedCommand>
    {
        private readonly GameDataSystem _gameDataSystem;
        private readonly CurrencyService _currencyService;
        private readonly CarController _carController;
        private readonly ReceiverManager _receiverManager;
        private readonly ContainerManager _containerManager;

        public UpgradePopupHandler
        (GameDataSystem gameDataSystem, CurrencyService currencyService,
            CarController carController, ReceiverManager receiverManager, 
            ContainerManager containerManager)
        {
            _gameDataSystem = gameDataSystem;
            _currencyService = currencyService;
            _carController = carController;
            _receiverManager = receiverManager;
            _containerManager = containerManager;
        }

        public Task<Unit> Handle(UpgradePopupOperations.ContainerBoughtCommand request, CancellationToken cancellationToken)
        {
            _currencyService.SubtractMoney(request.ContainerCost);
            _containerManager.StartWarmUpOfContainer();
            _gameDataSystem.SetMoneyData(_currencyService.Money.CurrentValue);
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpgradePopupOperations.CompanyPromotedCommand request, CancellationToken cancellationToken)
        {
            _currencyService.SubtractMoney(request.PromotionCost);
            _receiverManager.UpdateMaxNumberOfReceivers();
            _gameDataSystem.SetMaxNumberOfOrdersData(_receiverManager.MaxDemandingReceivers);
            _gameDataSystem.SetMoneyData(_currencyService.Money.CurrentValue);
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(UpgradePopupOperations.CapacityIncreasedCommand request, CancellationToken cancellationToken)
        {
            _currencyService.SubtractMoney(request.CapacityUpgradeCost);
            _carController.UpdateCarCapacity();
            _gameDataSystem.SetCapacityData(_carController.Capacity);
            _gameDataSystem.SetMoneyData(_currencyService.Money.CurrentValue);
            return Task.FromResult(Unit.Value);
        }
    }
}