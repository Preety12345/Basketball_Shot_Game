using Assets.Game.Scripts;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public MobileJoystick joystick;
    public float speed = 4f;
    CharacterController cc;
    float vy;

    void Awake() => cc = GetComponent<CharacterController>();

    void Update()
    {
        Vector3 move = transform.right * joystick.Direction.x + transform.forward * joystick.Direction.y;
        vy = cc.isGrounded ? -1f : vy + Physics.gravity.y * Time.deltaTime;
        cc.Move((move * speed + Vector3.up * vy) * Time.deltaTime);
    }
}