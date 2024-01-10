using UniRx;

namespace Services
{
    public class MoneyService :  IMoneyService
    {
        public IReadOnlyReactiveProperty<int> Money => _money;
        
        private readonly ReactiveProperty<int> _money = new();
        
        public void AddMoney(int amountMoney)
        {
            _money.Value += amountMoney;
        }

        public void SubtractMoney(int amountMoney)
        {
            _money.Value -= amountMoney;
        }
    }
}