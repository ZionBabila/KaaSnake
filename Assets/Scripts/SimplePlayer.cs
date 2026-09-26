// using System.Numerics;
using UnityEngine;
using UnityEngine.InputSystem;
using Vector2 = UnityEngine.Vector2;

public class SimplePlayer : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed = 1f;
    public PlayerDetect detect;
    public UnityEngine.Vector2 move;
    public UnityEngine.Vector2 jump;
    public bool Grounded = false;
    public float jumpTime = 0.5f;
    public float jumpForce = 100f;
    public float radius = 1f;
    public float Raylength = 1f;
    public float jumpThreshold =  0.8f;
    public float verticalMove = 0;
    float Timer = 0;
    public AudioSource JumpSound;
    public InputAction MoveAction;
    public InputAction HypnotizeAction;
    public InputAction Jump;
public bool canMove = true;
    private void Start()
    {
        
        rb = GetComponent<Rigidbody2D>();
        detect = GetComponentInChildren<PlayerDetect>();
        MoveAction.Enable();
        Jump.Enable();
        HypnotizeAction.Enable();
    }

 private void Update()
{
    GroundCheck();

    if (!canMove)
    {
        verticalMove = 0;
        jump = Vector2.zero;
        Timer = 0;
        return;
    }

    verticalMove = MoveAction.ReadValue<float>();

    if (Jump.WasPressedThisFrame() && Grounded)
    {
        Timer = jumpTime;
        if (JumpSound != null && JumpSound.isPlaying == false)
        {
            JumpSound.Play();
        }
    }
    if (Timer > 0)
    {
        jump = new Vector2(0, 1 * jumpForce);
        Timer = Timer - Time.deltaTime;
    }
    else
    {
        jump = Vector2.zero;
    }
}
    private void FixedUpdate()
    {
        rb.AddForce(new Vector2(verticalMove * speed, 0) + jump, ForceMode2D.Force);
    }
    public void GroundCheck()
    {
        if (Physics2D.CircleCast(transform.position, radius, UnityEngine.Vector2.down, Raylength) == true)
        {
            Grounded = true;

        }
        else
        {
            Grounded = false;

        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new UnityEngine.Vector3(0, -Raylength, 0), radius);
    }
}

