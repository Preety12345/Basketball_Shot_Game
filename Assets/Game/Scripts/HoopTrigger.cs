using UnityEngine;

namespace Assets.Game.Scripts
{
    public class HoopTrigger : MonoBehaviour
    {
        [SerializeField] private Hoop hoop;
        [SerializeField] private bool isTop;

        void OnTriggerEnter(Collider other)
        {
            if (isTop) hoop.OnTop(other); else hoop.OnBottom(other);
        }

        void OnTriggerExit(Collider other)
        {
            if (isTop) hoop.OnTopExit(other, transform.position.y);
        }
    }
}