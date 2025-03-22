using MediatR;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService
{
    public static class CurrencyServiceOperations
    {
        public record CheckSufficientFunds(int Money) : IRequest<bool>
        {
            public int Money { get; } = Money;
        }
    }
}