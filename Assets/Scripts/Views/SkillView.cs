using Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Views
{
    public class SkillView : MonoBehaviour
    {
        public Button Button => _button;
        public TextMeshProUGUI Text => _text;

        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _text;
        [SerializeField] private Image _image;
        [SerializeField] private float _alpha;

            [Header("Skill color setting")] [SerializeField]
        private Color _learnedColor = Color.red;

        [SerializeField] private Color _availableColor = Color.blue;

        private void Start()
        {
            _image.alphaHitTestMinimumThreshold =_alpha;
            _button.interactable = false;
        }

        public void UpdateButton(SkillState state)
        {
            UpdateColor(state);
            UpdateInteractable(state);
        }

        private void UpdateColor(SkillState state)
        {
            var colors = _button.colors;

            switch (state)
            {
                case SkillState.Bought:
                    colors.normalColor = _learnedColor;
                    break;

                case SkillState.Available:
                    colors.normalColor = _availableColor;
                    break;
            }

            _button.colors = colors;
        }

        private void UpdateInteractable(SkillState state)
        {
            _button.interactable = state switch
            {
                SkillState.Bought => true,
                SkillState.Available => true,
                SkillState.Unavailable => false,
                _ => _button.interactable
            };
        }
    }
}