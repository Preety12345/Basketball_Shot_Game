using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Game.Scripts
{
    public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public RectTransform background, handle;
        public Vector2 Direction { get; private set; }

        public void OnPointerDown(PointerEventData e) => OnDrag(e);

        public void OnDrag(PointerEventData e)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(background, e.position, e.pressEventCamera, out var p);
            float r = background.sizeDelta.x * 0.5f;
            Direction = Vector2.ClampMagnitude(p / r, 1f);
            handle.anchoredPosition = Direction * r;
        }

        public void OnPointerUp(PointerEventData e)
        {
            Direction = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
        }
    }
}