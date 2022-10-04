using Events;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Views
{
    public class BackgroundView : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            MessageBroker.Default.Publish(new CurrentSkillSelectedEvent(null));
        }
    }
}