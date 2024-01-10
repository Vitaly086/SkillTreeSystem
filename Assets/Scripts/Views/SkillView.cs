using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class SkillView : MonoBehaviour
    {
        public Button Button => _button;

        [SerializeField]
        private Button _button;
        [SerializeField]
        private TextMeshProUGUI _text;
        [SerializeField]
        private Image _image;
        [SerializeField]
        private Image _frame;
        [SerializeField]
        private Color _boughtFrameColor;
        [SerializeField]
        private Color _availableFrameColor;
        [SerializeField]
        private Color _unavailableFrameColor;


        public void Initialize(Sprite sprite)
        {
            _image.sprite = sprite;
            _button.interactable = false;
        }

        public void UpdateButton(SkillState state)
        {
            UpdateInteractable(state);
            UpdateFrameColor(state);
        }

        public void SetPrice(string price)
        {
            _text.text = price;
        }

        private void UpdateFrameColor(SkillState state)
        {
            _frame.color = state switch
            {
                SkillState.Bought => _boughtFrameColor,
                SkillState.Available => _availableFrameColor,
                SkillState.Unavailable => _unavailableFrameColor,
                _ => _frame.color
            };
        }

        private void UpdateInteractable(SkillState state)
        {
            _button.interactable = state != SkillState.Unavailable;
        }
    }
}