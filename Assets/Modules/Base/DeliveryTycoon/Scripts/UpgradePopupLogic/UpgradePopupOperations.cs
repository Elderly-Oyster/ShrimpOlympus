using MediatR;

namespace Modules.Base.DeliveryTycoon.Scripts.UpgradePopupLogic
{
    public static class UpgradePopupOperations
    {
        public record ContainerBoughtCommand(int ContainerCost) : IRequest
        {
            public int ContainerCost { get; } = ContainerCost;
        }

        public record CompanyPromotedCommand(int PromotionCost) : IRequest
        {
            public int PromotionCost { get; } = PromotionCost;
        }

        public record CapacityIncreasedCommand(int CapacityUpgradeCost) : IRequest
        {
            public int CapacityUpgradeCost { get; } = CapacityUpgradeCost;
        }
    }
}