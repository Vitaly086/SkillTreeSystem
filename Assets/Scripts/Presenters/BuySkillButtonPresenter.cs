using Events;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Presenters
{
    public class BuySkillButtonPresenter : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private SkillPresenter _selectedSkill;

        private void Start()
        {
            _button.interactable = false;

            MessageBroker.Default.Receive<CurrentSkillSelectedEvent>()
                .Subscribe(eventData =>
                {
                    _selectedSkill = eventData.CurrentPresenter;
                    _button.interactable = eventData.CanBuy;
                    
                })
                .AddTo(this);


            _button.OnClickAsObservable()
                .Subscribe(_ => { _selectedSkill?.Buy(); })
                .AddTo(this);
        }
    }
}