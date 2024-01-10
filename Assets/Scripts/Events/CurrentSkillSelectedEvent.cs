using Presenters;

namespace Events
{
    public class CurrentSkillSelectedEvent
    {
        public SkillPresenter CurrentPresenter { get; }
        public bool CanBuy { get; }
        public bool CanSell { get; }

        public CurrentSkillSelectedEvent(SkillPresenter currentPresenter, bool canBuy = false, bool canSell = false)
        {
            CurrentPresenter = currentPresenter;
            CanBuy = canBuy;
            CanSell = canSell;
        }
    }
}