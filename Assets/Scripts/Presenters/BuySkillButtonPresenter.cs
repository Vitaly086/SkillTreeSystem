using Events;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Presenters
{
    public class BuySkillButtonPresenter : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private SkillPresenter _selectedPresenter;

        private void Start()
        {
            _button.interactable = false;

            MessageBroker.Default.Receive<SelectCurrentPresenterEvent>()
                .Subscribe(eventData =>
                {
                    _selectedPresenter = eventData.CurrentPresenter;
                    _button.interactable = eventData.IsCanBuy;
                    
                })
                .AddTo(this);


            _button.OnClickAsObservable()
                .Subscribe(_ => { _selectedPresenter?.Buy(); })
                .AddTo(this);
        }
    }
}