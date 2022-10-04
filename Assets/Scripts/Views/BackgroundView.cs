using Events;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class BackgroundView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private SpriteRenderer _sprite;

        private void Awake()
        {
            var size = new Vector2(Screen.width, Screen.height);
            _sprite.size = size;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            MessageBroker.Default.Publish(new CurrentSkillSelectedEvent(null));
        }
    }
}