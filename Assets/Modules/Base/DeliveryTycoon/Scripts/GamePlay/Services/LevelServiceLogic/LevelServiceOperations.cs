using MediatR;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.LevelServiceLogic
{
    public static class LevelServiceOperations
    {
        public record NewUpgradeUnlockedCommand(int NumberOfUnlockedUpgrades) : IRequest
        {
            public int NumberOfUnlockedUpgrades { get; } = NumberOfUnlockedUpgrades;
        }

        public record LevelUpCommand(int Level) : IRequest
        {
            public int Level { get; } = Level;
        }
        
    }
}