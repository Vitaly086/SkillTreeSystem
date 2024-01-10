using Services;
using TMPro;
using UniRx;
using UnityEngine;

namespace Presenters
{
    public class UserMoneyPresenter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _userMoneyValue;


        private void Start()
        {
            var moneyService = ServiceLocator.Instance.GetSingle<IMoneyService>();
        
            moneyService.Money
                .Subscribe(_ => _userMoneyValue.text = $"{moneyService.Money.Value} $")
                .AddTo(this);
        }
    }
}