using Presenters;

namespace Events
{
    public class SelectCurrentPresenterEvent
    {
        public SkillPresenter CurrentPresenter { get; }
        public bool IsCanBuy { get; }
        public bool IsCanSell { get; }

        public SelectCurrentPresenterEvent(SkillPresenter currentPresenter)
        {
            CurrentPresenter = currentPresenter;
        }
        public SelectCurrentPresenterEvent(SkillPresenter currentPresenter, bool isCanBuy, bool isCanSell)
        {
            CurrentPresenter = currentPresenter;
            IsCanBuy = isCanBuy;
            IsCanSell = isCanSell;
        }
    }
}