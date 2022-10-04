using Presenters;

namespace Events
{
    public class CurrentSkillSelectedEvent
    {
        public SkillPresenter CurrentPresenter { get; }
        public bool CanBuy { get; }
        public bool CanSell { get; }

        public CurrentSkillSelectedEvent(SkillPresenter currentPresenter)
        {
            CurrentPresenter = currentPresenter;
        }
        public CurrentSkillSelectedEvent(SkillPresenter currentPresenter, bool canBuy, bool canSell)
        {
            CurrentPresenter = currentPresenter;
            CanBuy = canBuy;
            CanSell = canSell;
        }
    }
}