using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService;
using Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService;
using static Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Cars.BaseCarControllerOperations;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Cars
{
    public class BaseCarControllerHandler : 
        IRequestHandler<MoneyObtainedCommand>,
        IRequestHandler<ExperienceObtained>
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
        
        public Task Handle(MoneyObtainedCommand request, CancellationToken cancellationToken)
        {
            _currencyService.AddMoney(request.Amount);
            _gameDataSystem.SetMoneyData(_currencyService.Money.CurrentValue);
            return Task.FromResult(Unit.Value);
        }

        public Task Handle(ExperienceObtained request, CancellationToken cancellationToken)
        {
            _levelService.GetExperience(request.Experience);
            _gameDataSystem.SetExperience(_levelService.Experience);
            return Task.FromResult(Unit.Value);
        }
    }
}