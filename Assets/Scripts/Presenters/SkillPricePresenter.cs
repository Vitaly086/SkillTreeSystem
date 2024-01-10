using Events;
using Models;
using TMPro;
using UniRx;
using UnityEngine;

namespace Presenters
{
    public class SkillPricePresenter : MonoBehaviour
    {
        private const string SKILL_NOT_SELECTED_TEXT = "Skill not selected";
        private const string SKILL_BOUGHT_TEXT_FORMAT = "{0} is bought";
        private const string SKILL_COST_TEXT_FORMAT = "{0} costs: {1} money";

        [SerializeField]
        private TextMeshProUGUI _skillPrice;

        private void Start()
        {
            _skillPrice.text = SKILL_NOT_SELECTED_TEXT;

            MessageBroker.Default.Receive<CurrentSkillSelectedEvent>()
                .Where(eventData => eventData.CurrentPresenter != null)
                .Select(eventData => eventData.CurrentPresenter)
                .Subscribe(UpdateSkillPriceText)
                .AddTo(this);
        }

        private void UpdateSkillPriceText(SkillPresenter currentPresenter)
        {
            if (currentPresenter == null)
            {
                _skillPrice.text = SKILL_NOT_SELECTED_TEXT;
                return;
            }

            _skillPrice.text = currentPresenter.State.Value == SkillState.Bought
                ? string.Format(SKILL_BOUGHT_TEXT_FORMAT, currentPresenter.SkillName)
                : string.Format(SKILL_COST_TEXT_FORMAT, currentPresenter.SkillName, currentPresenter.Cost);
        }
    }
}