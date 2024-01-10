using Events;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Presenters
{
    public class SellSkillButtonPresenter : MonoBehaviour
    {
        [SerializeField]
        private Button _button;

        private SkillPresenter _selectedSkill;

        private void Start()
        {
            _button.interactable = false;

            MessageBroker.Default.Receive<CurrentSkillSelectedEvent>()
                .Subscribe(HandleSkillSelected)
                .AddTo(this);


            _button.OnClickAsObservable()
                .Where(_ => _selectedSkill != null)
                .Subscribe(_ => _selectedSkill.SellSkill())
                .AddTo(this);
        }

        private void HandleSkillSelected(CurrentSkillSelectedEvent eventData)
        {
            _selectedSkill = eventData.CurrentPresenter;
            _button.interactable = eventData.CanSell;
        }
    }
}