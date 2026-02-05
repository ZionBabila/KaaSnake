using UnityEngine;

public class MonkeyEnemyAI : MonoBehaviour
{
    [Header("Components")]
    public Animator anim;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public LayerMask playerLayer;

    [Header("Detection & Scanning")]
    public float detectionRange = 7f;
    public float sideDirection = 1f;
    public float scanSpeed = 2f;
    public float minAngle = -0.8f;
    public float maxAngle = -0.2f;

    [Header("Organic Feel")]
    public float lockOnDuration = 1.5f;
    private float lastSeenTime;
    private bool hasEverSeenPlayer = false;

    [Header("Throw Settings")]
    public float throwPower = 12f;
    public float arcHeight = 5f;
    public float fireRate = 1.5f;

    private float nextFireTime;
    private Vector2 lastStoredDir; // נשמור את הכיוון האחרון שהיה בסריקה

    void Start()
    {
        lastSeenTime = -lockOnDuration;
    }

    void Update()
    {
        float scanStep = Mathf.PingPong(Time.time * scanSpeed, 1f);
        float currentAngle = Mathf.Lerp(minAngle, maxAngle, scanStep);
        Vector2 scanDir = new Vector2(sideDirection, currentAngle).normalized;
        lastStoredDir = scanDir; // מעדכנים כל הזמן את הכיוון לזריקה העתידית

        RaycastHit2D hit = Physics2D.Raycast(firePoint.position, scanDir, detectionRange, playerLayer);

        if (hit.collider != null && hit.collider.CompareTag("Player"))
        {
            lastSeenTime = Time.time;
            hasEverSeenPlayer = true;
        }

        bool isLockOn = hasEverSeenPlayer && (Time.time - lastSeenTime < lockOnDuration);

        Color rayColor = isLockOn ? Color.red : Color.green;
        Debug.DrawRay(firePoint.position, scanDir * detectionRange, rayColor);
       
        if (isLockOn)
        {
            if (anim != null) anim.SetBool("throw", true);

            // כאן השינוי: אנחנו לא קוראים לזריקה מכאן, אלא רק מנהלים את קצב האש
            if (Time.time >= nextFireTime)
            {
                // כאן אפשר להפעיל Trigger אם אתה עובד עם טריגר, 
                // או פשוט לתת לאנימציה לרוץ בלופ כל עוד ה-Bool הוא true
                nextFireTime = Time.time + fireRate;
            }
        }
        else
        {
            if (anim != null) anim.SetBool("throw", false);
        }
    }

    // --- הפונקציה הזו תיקרא מהאנימציה! ---
    public void ExecuteThrowEvent()
    {
        GameObject banana = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = banana.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // משתמשים בכיוון האחרון שנשמר בסריקה
            Vector2 throwVelocity = new Vector2(lastStoredDir.x * throwPower, (lastStoredDir.y * throwPower) + arcHeight);
            rb.linearVelocity = throwVelocity;
            rb.AddTorque(Random.Range(-10f, 10f), ForceMode2D.Impulse);
        }
    }
    private void OnDrawGizmos()
{
    if (firePoint == null) return;

    Gizmos.color = Color.yellow;
    // Draw the two boundaries of the monkey's vision
    Vector2 minDir = new Vector2(sideDirection, minAngle).normalized;
    Vector2 maxDir = new Vector2(sideDirection, maxAngle).normalized;

    Gizmos.DrawRay(firePoint.position, minDir * detectionRange);
    Gizmos.DrawRay(firePoint.position, maxDir * detectionRange);
    
    // Draw a small circle at the fire point
    Gizmos.DrawWireSphere(firePoint.position, 0.2f);
}
}