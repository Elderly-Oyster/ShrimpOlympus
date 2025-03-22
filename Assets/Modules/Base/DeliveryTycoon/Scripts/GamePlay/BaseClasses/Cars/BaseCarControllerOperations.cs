using MediatR;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.BaseClasses.Cars
{
    public static class BaseCarControllerOperations
    {
        public record MoneyObtainedCommand(int Amount) : IRequest
        {
            public int Amount { get; } = Amount;
        }

        public record ExperienceObtained(int Experience) : IRequest
        {
            public int Experience { get; } = Experience;
        }
    }
}