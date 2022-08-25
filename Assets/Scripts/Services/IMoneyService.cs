using UniRx;

namespace Services
{
    public interface IMoneyService : IService
    {
        public IReadOnlyReactiveProperty<int> Money { get; }
        public void AddMoney(int amountMoney);
        public void SubtractMoney(int amountMoney);
    }
}