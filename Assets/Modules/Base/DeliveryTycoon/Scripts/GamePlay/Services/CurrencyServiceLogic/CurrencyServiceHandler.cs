using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyServiceLogic
{
    public class CurrencyServiceHandler : IRequestHandler<CurrencyServiceOperations.CheckSufficientFunds, bool>
    {
        private readonly CurrencyService _currencyService;

        public CurrencyServiceHandler(CurrencyService currencyService)
        {
            _currencyService = currencyService;
        }


        public Task<bool> Handle(CurrencyServiceOperations.CheckSufficientFunds request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_currencyService.CheckMoney(request.Money));
        }
    }
}