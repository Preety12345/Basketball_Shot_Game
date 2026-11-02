using TMPro;
using UnityEngine;

namespace Assets.Game.Scripts
{
    public class Hoop : MonoBehaviour
    {
        [SerializeField]private float maxPassTime = 1f;// top → bottom must happen within this time
        [SerializeField]private TextMeshProUGUI txt_score;
        [SerializeField]private int pointsPerBasket = 2;

        int score;
        bool ballAbove;
        float aboveTime;


        public void OnTop(Collider c)
        {
            if (!c.CompareTag("Ball")) return;
            ballAbove = true;
            aboveTime = Time.time;
        }

        // If the ball leaves the top trigger upward, it bounced out
        public void OnTopExit(Collider c, float triggerY)
        {
            if (c.CompareTag("Ball") && c.transform.position.y > triggerY) ballAbove = false;
        }

        public void OnBottom(Collider c)
        {
            if (!c.CompareTag("Ball")) return;
            if (ballAbove && Time.time - aboveTime <= maxPassTime)
            {
                score += pointsPerBasket;
                if (txt_score) txt_score.text = "Score: " + score;
            }
            ballAbove = false;
        }
    }
}