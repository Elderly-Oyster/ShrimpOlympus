using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystemLogic;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelServiceLogic
{
    public class LevelServiceHandler :
        IRequestHandler<LevelServiceOperations.NewUpgradeUnlockedCommand>, 
        IRequestHandler<LevelServiceOperations.LevelUpCommand>
    {
        private readonly GameDataSystem _gameDataSystem;

        public LevelServiceHandler(GameDataSystem gameDataSystem)
        {
            _gameDataSystem = gameDataSystem;
        }
        
        public Task<Unit> Handle(LevelServiceOperations.NewUpgradeUnlockedCommand request, CancellationToken cancellationToken)
        {
            _gameDataSystem.SetNumberOfUnlockedUpgradesData(request.NumberOfUnlockedUpgrades);
            return Task.FromResult(Unit.Value);
        }

        public Task<Unit> Handle(LevelServiceOperations.LevelUpCommand request, CancellationToken cancellationToken)
        {
            _gameDataSystem.SetLevelData(request.Level);
            return Task.FromResult(Unit.Value);
        }
    }
}