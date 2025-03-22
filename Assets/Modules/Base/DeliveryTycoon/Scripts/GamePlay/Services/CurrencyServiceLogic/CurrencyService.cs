using R3;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Services.CurrencyService
{
    public class CurrencyService
    {
        private readonly ReactiveProperty<int> _money = new();
        
        public ReadOnlyReactiveProperty<int> Money => _money;

        public void Initialize(int money) => _money.Value = money;
        
        public void AddMoney(int amount) => _money.Value += amount;

        public void SubtractMoney(int amount) => _money.Value -= amount;

        public bool CheckMoney(int amount) => _money.CurrentValue >= amount;
    }
}