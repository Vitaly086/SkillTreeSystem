using Events;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Presenters
{
    public class SellAllSkillButtonPresenter : MonoBehaviour
    {
        [SerializeField] private Button _button;

        private void Start()
        {
            _button.OnClickAsObservable()
                .Subscribe(_ => MessageBroker.Default.Publish(new SellAllSkillsEvent()))
                .AddTo(this);
        }
    }
}