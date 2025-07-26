using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MediatR;
using Modules.Base.DeliveryTycoon.Scripts.DataSaving.GameDataSystem;
using static Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService.LevelServiceOperations;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelService
{
    public class LevelServiceHandler :
        IRequestHandler<NewUpgradeUnlockedCommand>, 
        IRequestHandler<LevelUpCommand>
    {
        private readonly GameDataSystem _gameDataSystem;

        public LevelServiceHandler(GameDataSystem gameDataSystem) => 
            _gameDataSystem = gameDataSystem;

        public Task Handle([NotNull] NewUpgradeUnlockedCommand request, CancellationToken cancellationToken)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            _gameDataSystem.SetNumberOfUnlockedUpgradesData(request.NumberOfUnlockedUpgrades);
            return Task.FromResult(Unit.Value);
        }

        public Task Handle(LevelUpCommand request, CancellationToken cancellationToken)
        {
            _gameDataSystem.SetLevelData(request.Level);
            return Task.FromResult(Unit.Value);
        }
    }
}