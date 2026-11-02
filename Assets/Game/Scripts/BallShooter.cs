using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using TMPro;

public class BallShooter : MonoBehaviour
{
    [HorizontalLine(color: EColor.Yellow)]
    [Header("Assignables  :   References")]
    [SerializeField]private Transform hand;
    [SerializeField]private Rigidbody ball;
    [SerializeField]private Camera mainCam;
    [SerializeField]private LineRenderer line;
    [SerializeField] private TextMeshProUGUI txt_angle;
    [SerializeField] private TextMeshProUGUI txt_speed;
    [SerializeField]private LayerMask hitMask = ~0;// exclude the player and ball layer

    [HorizontalLine(color: EColor.Red)]
    [Header("Assignables  :   Aim Settings")]
    [SerializeField]private float yawSensitivity = 0.15f;    // degrees per pixel
    [SerializeField]private float pitchSensitivity = 0.10f;
    [SerializeField]private float minAngle = 25f;
    [SerializeField]private float maxAngle = 75f;

    [HorizontalLine(color: EColor.Yellow)]
    [Header("Assignables  :   Shoot Power")]
    [SerializeField]private float minPower = 4f;
    [SerializeField]private float maxPower = 14f;
    [SerializeField]private float chargeTime = 2.2f; // seconds from min to max

    [HorizontalLine(color: EColor.Blue)]
    [Header("Assignables  :   Other Values")]
    [SerializeField]private float ballPickupRange = 2f;
    [SerializeField]private float maxCameraPitch = 25f;
    [SerializeField]private float ballRespawnDelay = 4f;

    [HideInInspector] public bool dragging;

    private float angle = 45f;
    private const float Dx = 0.15f;
    private float power;
    private bool charging, holding;
    private Collider ballCol;
    readonly Vector3[] pts = new Vector3[120];
    private Coroutine respawnRoutine;

    #region Unity:-

    private void Awake()
    {
        ballCol = ball.GetComponent<Collider>();
        line.enabled = false;
        power = minPower;
    }

    private void Update()
    {
        if (charging)
        {
            power = Mathf.MoveTowards(
                power, maxPower, (maxPower - minPower) / chargeTime * Time.deltaTime);
            txt_speed.text = $"Speed: {power:F0} m/s";
        }

        bool aiming = holding && (dragging || charging);
        line.enabled = aiming;
        if (aiming) DrawPath();
    }

    void LateUpdate()
    {
        //For camera tilting visual effect
        float t = Mathf.InverseLerp(minAngle, maxAngle, angle);
        float pitch = -Mathf.Lerp(0f, maxCameraPitch, t);
        mainCam.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    #endregion

    #region Ball Charge and Release:-

    public void AddDrag(Vector2 d)
    {
        //Calculating the launch angle by dragging up/down
        transform.Rotate(0f, d.x * yawSensitivity, 0f);
        angle = Mathf.Clamp(angle + d.y * pitchSensitivity, minAngle, maxAngle);
        txt_angle.text = $"Current Angle: {angle:F0}";
        Debug.Log($"Current angle :{angle}");
    }

    // Shooting
    public void BeginCharge()
    {
        if (!holding) return;
        charging = true;
        power = minPower;
    }

    public void Release()
    {
        if (!charging) return;
        charging = false;
        if (!holding) return;

        holding = false;
        ball.transform.SetParent(null);
        ballCol.enabled = true;
        ball.isKinematic = false;
        ball.linearVelocity = LaunchVelocity();
        line.enabled = false;
        power = minPower;

        //start respawning the ball
        if (respawnRoutine != null) StopCoroutine(respawnRoutine);
        respawnRoutine = StartCoroutine(RespawnAfterDelay());
    }

    #endregion

    private IEnumerator RespawnAfterDelay()
    {
        yield return new WaitForSeconds(ballRespawnDelay);

        //Stop the ball's motion before teleporting
        ball.linearVelocity = Vector3.zero;
        ball.angularVelocity = Vector3.zero;

        //set ball position beside the player after respawn
        ball.position = transform.position + transform.forward * 2.5f
            + Vector3.up * 0.3f;

        respawnRoutine = null;
    }

    // Ball pickup, callable from button
    public void TryPickup()
    {
        //If the ball is not within pickup range then return
        if (holding) return;
        if ((ball.position - transform.position).sqrMagnitude > ballPickupRange * ballPickupRange) return;

        holding = true;
        ball.isKinematic = true;
        ballCol.enabled = false;
        ball.transform.SetParent(hand);
        ball.transform.localPosition = Vector3.zero;
    }

    // Launch math
    private Vector3 LaunchVelocity()
    {
        //Flatten the forward vector to remove Vy (Velocity from Y axis)
        //Find the direction the player is facing in ground
        Vector3 fwd = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 dir = Quaternion.AngleAxis(-angle, transform.right) * fwd;   // tilt upward by angle
        return dir * power;
    }

    private void DrawPath()
    {
        Vector3 p0 = hand.position;
        Vector3 flat = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;

        float th = angle * Mathf.Deg2Rad;
        float tan = Mathf.Tan(th);
        float cos = Mathf.Cos(th);
        float g = Mathf.Abs(Physics.gravity.y);
        float k = g / (2f * power * power * cos * cos);   // the x² coefficient, constant for the whole arc

        int n = 0;
        pts[n++] = p0;
        Vector3 prev = p0;

        for (int i = 1; i < pts.Length; i++)
        {
            float x = i * Dx;
            float y = x * tan - k * x * x;                // y(x) = x·tanθ − g·x² / (2v²cos²θ)
            Vector3 p = p0 + flat * x + Vector3.up * y;

            if (Physics.Linecast(prev, p, out var hit, hitMask, QueryTriggerInteraction.Ignore))
            {
                pts[n++] = hit.point;
                break;
            }

            pts[n++] = p;
            prev = p;
        }

        line.positionCount = n;
        line.SetPositions(pts);
    }
}