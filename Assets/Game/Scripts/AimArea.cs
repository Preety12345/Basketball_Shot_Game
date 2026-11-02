
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Game.Scripts
{
    public class AimArea : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public BallShooter shooter;
        public void OnBeginDrag(PointerEventData e) => shooter.dragging = true;
        public void OnDrag(PointerEventData e) => shooter.AddDrag(e.delta);
        public void OnEndDrag(PointerEventData e) => shooter.dragging = false;
    }
}