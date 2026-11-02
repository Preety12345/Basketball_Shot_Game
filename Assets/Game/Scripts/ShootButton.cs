
namespace Assets.Game.Scripts
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    public class ShootButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        public BallShooter shooter;
        public void OnPointerDown(PointerEventData e) => shooter.BeginCharge();
        public void OnPointerUp(PointerEventData e) => shooter.Release();
    }
}