using Events;
using Models;
using TMPro;
using UniRx;
using UnityEngine;

namespace Presenters
{
    public class SkillPricePresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _skillPrice;

        private void Start()
        {
            _skillPrice.text = "Skill not selected";

            MessageBroker.Default.Receive<SelectCurrentPresenterEvent>()
                .Select(eventData => eventData.CurrentPresenter)
                .Subscribe(currentPresenter =>
                {
                    if (IsNull(currentPresenter)) return;

                    _skillPrice.text = currentPresenter.State.Value == SkillState.Bought
                        ? $"{currentPresenter.name} is bought"
                        : $"{currentPresenter.name} costs: {currentPresenter.Cost.ToString()} money";
                }).AddTo(this);
        }

        private bool IsNull(SkillPresenter presenter)
        {
            if (presenter == null)
            {
                _skillPrice.text = "Skill not selected";
                return true;
            }

            return false;
        }
    }
}