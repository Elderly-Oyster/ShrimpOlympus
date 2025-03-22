using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystemLogic;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyServiceLogic;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelServiceLogic;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Cars
{
    public class BaseCarControllerHandler : 
        IRequestHandler<BaseCarControllerOperations.MoneyObtainedCommand>,
        IRequestHandler<BaseCarControllerOperations.ExperienceObtained>
    {
        private readonly CurrencyService _currencyService;
        private readonly GameDataSystem _gameDataSystem;
        private readonly LevelService _levelService;

        public BaseCarControllerHandler(CurrencyService currencyService, GameDataSystem gameDataSystem, LevelService levelService)
        {
            _currencyService = currencyService;
            _gameDataSystem = gameDataSystem;
            _levelService = levelService;
        }
        
        public Task<Unit> Handle(BaseCarControllerOperations.MoneyObtainedCommand request, CancellationToken cancellationToken)
        {
            _currencyService.AddMoney(request.Amount);
            _gameDataSystem.SetMoneyData(_currencyService.Money.CurrentValue);
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(BaseCarControllerOperations.ExperienceObtained request, CancellationToken cancellationToken)
        {
            _levelService.GetExperience(request.Experience);
            _gameDataSystem.SetExperience(_levelService.Experience);
            return Task.FromResult(Unit.Value);
        }
    }
}