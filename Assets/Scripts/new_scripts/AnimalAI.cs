using UnityEngine;

public class AnimalAI : MonoBehaviour
{
    [Header("Animator Parameter Names")]
    public string attackTrigger = "attack";       // Trigger - מתי שהקוף זורק בננה
    public string stopAttackTrigger = "stop_attack";
    [Header("Detection")]
    public Transform firePoint;
    public LayerMask playerLayer;
    public float range = 7f;
    public float scanSpeed = 2f;
    public float sideDirection = 1f; // 1 for right, -1 for left
    public float minAngle = -0.8f;   // Lower bound of the scan cone
    public float maxAngle = -0.2f;   // Upper bound of the scan cone

    [Header("Shooting & Projectiles")]
    public GameObject projectilePrefab;
    public float fireRate = 1.5f;
    public float throwForce = 10f;    // Speed of the banana
    public float upwardArc = 2f;      // Gives the throw a little "lift"
    private float nextFireTime;

    private bool isDisabled = false;
    private Animator anim;
    private Vector2 lastScanDir;      // Remembers where we were looking when we fired

    [Header("Visual Debug")]
    public bool showScanCone = true;

    void Awake() => anim = GetComponent<Animator>();

    void Update()
    {
        if (isDisabled) return;
        HandleScanning();
    }

    private void HandleScanning()
    {
        // Calculate the moving scan beam
        float angle = Mathf.Lerp(minAngle, maxAngle, Mathf.PingPong(Time.time * scanSpeed, 1f));
        Vector2 dir = new Vector2(sideDirection, angle).normalized;
        lastScanDir = dir; // Store this for the projectile logic

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, dir, range, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            if (Time.time >= nextFireTime)
            {
                Attack();
            }
        }

        // Visual debug ray in Scene View
        Debug.DrawRay(firePoint.position, dir * range, hit.collider ? Color.red : Color.green);
    }

    private void Attack()
    {
        nextFireTime = Time.time + fireRate;
        if (anim != null) anim.SetTrigger(attackTrigger);
    }

    // --- NEW: THE MISSING FUNCTION CALLED BY ANIMATION EVENT ---
    public void ExecuteThrowEvent()
    {
        if (projectilePrefab == null || firePoint == null) return;

        Debug.Log(gameObject.name + " is throwing a projectile!");

        // 1. Create the projectile
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        
        // 2. Apply physics
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // We use the last direction we were scanning at
            Vector2 finalThrowForce = new Vector2(lastScanDir.x * throwForce, (lastScanDir.y * throwForce) + upwardArc);
            rb.AddForce(finalThrowForce, ForceMode2D.Impulse);

            // Add some random spin
            rb.AddTorque(Random.Range(-5f, 5f), ForceMode2D.Impulse);
        }
    }

    public void DisableAI()
    {
        isDisabled = true;
        if (anim != null) anim.SetTrigger(stopAttackTrigger); // Optional: clear any attack triggers
        Debug.Log(gameObject.name + " AI Disabled");
    }

    private void OnDrawGizmos()
    {
        if (!showScanCone || firePoint == null) return;

        Gizmos.color = Color.yellow;
        Vector2 topDir = new Vector2(sideDirection, maxAngle).normalized;
        Vector3 topEdge = firePoint.position + (Vector3)topDir * range;

        Vector2 bottomDir = new Vector2(sideDirection, minAngle).normalized;
        Vector3 bottomEdge = firePoint.position + (Vector3)bottomDir * range;

        Gizmos.DrawLine(firePoint.position, topEdge);
        Gizmos.DrawLine(firePoint.position, bottomEdge);
        Gizmos.DrawLine(topEdge, bottomEdge);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(firePoint.position, 0.15f);
    }
}