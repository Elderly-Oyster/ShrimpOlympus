using System.Collections.Generic;
using Core.MVP;

namespace Modules.Base.ConverterScreen.Scripts
{
    public enum Currencies
    {
        Eur,
        Usd,
        Pln,
        Pr
    }
    public class ConverterScreenModel : IScreenModel
    {

        private Currencies _sourceCurrency;
        private Currencies _targetCurrency;

        public ConverterScreenModel() { }

        private readonly Dictionary<Currencies, float> _currencyToEuroRate = new()
        {
            { Currencies.Eur, 1.0f },
            { Currencies.Usd, 1.1f },
            { Currencies.Pln, 4.6f },
            { Currencies.Pr, 0.05f }
        };

        public void SelectSourceCurrency(Currencies currency) => _sourceCurrency = currency;
        public void SelectTargetCurrency(Currencies currency) => _targetCurrency = currency;

        public float ConvertSourceToTarget(float amount) =>
            ConvertCurrency(amount, _sourceCurrency, _targetCurrency);

        public float ConvertTargetToSource(float amount) =>
            ConvertCurrency(amount, _targetCurrency, _sourceCurrency);
        
        private float ConvertCurrency(float amount, Currencies from, Currencies to)
        {
            var amountInEuro = amount / _currencyToEuroRate[from];
            var convertedAmount = amountInEuro * _currencyToEuroRate[to];
            return convertedAmount;
        }
        
        public void Dispose()
        { 
            
        }
    }
}