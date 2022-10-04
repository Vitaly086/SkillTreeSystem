using Events;
using Services;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Presenters
{
    public class MoneyButtonPresenter : MonoBehaviour
    {
        [SerializeField] private int _amountMoney;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;

        private void Start()
        {
            var moneyService = ServiceLocator.Instance.GetSingle<IMoneyService>();

            _text.text = _amountMoney >= 0 ? $"Add {_amountMoney} money" : $"Subtract {_amountMoney} money";

            _button.OnClickAsObservable()
                .Subscribe(_ =>
                {
                    MessageBroker.Default.Publish(new CurrentSkillSelectedEvent(null));
                    moneyService.AddMoney(_amountMoney);
                })
                .AddTo(this);
        }
    }
}