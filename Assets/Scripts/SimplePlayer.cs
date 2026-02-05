using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

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
    public float V;
    public float up;
    float Timer = 0;
    public AudioSource JumpSound;
    [Header("Camera Settings")]
    public Camera mainCam;
    public float cameraOffsetX = 5f;
    public float cameraSmoothTime = 3f;
    private float targetX;
    private void Start()
    {
        if (mainCam == null)
        {
            mainCam = GetComponentInChildren<Camera>();
        }
        targetX = cameraOffsetX;
        rb = GetComponent<Rigidbody2D>();
        detect = GetComponentInChildren<PlayerDetect>();
    }

    private void Update()
    {
        up = Input.GetAxisRaw("Vertical");

        V = Input.GetAxis("Horizontal");
        //loat H = Input.GetAxis("Vertical");

        // 1. Decide target side
        if (V > 0) targetX = cameraOffsetX;
        else if (V < 0) targetX = -cameraOffsetX;

        // 2. Get current local position using the Unity specific Vector3
        UnityEngine.Vector3 camPos = mainCam.transform.localPosition;

        // 3. Smooth the X movement
        float newX = Mathf.Lerp(camPos.x, targetX, Time.deltaTime * cameraSmoothTime);

        // 4. Apply back to the camera using the Unity specific Vector3
        mainCam.transform.localPosition = new UnityEngine.Vector3(newX, camPos.y, camPos.z);
        move = new UnityEngine.Vector2(V, 0);

        if (Input.GetAxisRaw("Vertical") >= 1 && Grounded == true)
        {
            Timer = jumpTime;
            if (JumpSound != null && JumpSound.isPlaying == false)
            {
                JumpSound.Play();
            }
        }
        if (Timer > 0)
        {
            jump = new UnityEngine.Vector2(0, 1 * jumpForce);
            Timer = Timer - Time.deltaTime;
        }
        else
        {
            jump = UnityEngine.Vector2.zero;
        }
        GroundCheck();
    }
    private void FixedUpdate()
    {
        rb.AddForce(move * speed + jump, ForceMode2D.Force);
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

