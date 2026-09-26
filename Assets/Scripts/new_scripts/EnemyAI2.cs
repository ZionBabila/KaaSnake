using UnityEngine;

public class EnemyAI2 : MonoBehaviour
{
    public enum AttackType { RangedShooter, MeleeChaser }

    [Header("Behavior Settings")]
    public AttackType attackType = AttackType.MeleeChaser;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;
    public bool moveRightFirst = true;

    [Header("Chasing / Aggro Settings")]
    public float chaseSpeed = 4f;        
    public float detectionRange = 6f;    

    [Header("Shooting Settings")]
    public MonoBehaviour spawnScript; 

    [Header("Vertical Floating")]
    public bool useFloating = true;
    public float floatAmplitude = 0.15f;
    public float floatSpeed = 2.5f;

    [Header("References")]
    public Animator animator;
    public HypnotizableEntity hypnosisSystem;
    public bool showGizmos = true;

    // Internal state variables
    private Vector3 startPosition;
    private float horizontalOffset = 0f;
    private int direction = 1;
    private Transform playerTarget;
    private bool isAggro = false; 

    void Start()
    {
        startPosition = transform.position;
        direction = moveRightFirst ? 1 : -1;

        if (hypnosisSystem == null)
            hypnosisSystem = GetComponent<HypnotizableEntity>();

        if (animator == null)
            animator = GetComponent<Animator>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTarget = p.transform;

        if (spawnScript != null) spawnScript.enabled = false;
    }

    void Update()
    {
        // --- התיקון הקריטי ---
        // 1. האם אני מהופנט כרגע?
        // אם כן: תעצור הכל (return).
        // אם לא (כלומר התאוששתי): תמשיך שורות למטה ותזוז.
        if (hypnosisSystem != null && hypnosisSystem.isActionCompleted) 
        {
            // (אופציונלי) כאן אפשר לוודא שהאנימציות כבויות בזמן שהוא מהופנט
            return;
        }

        // 2. מכאן והלאה - קוד התנועה הרגיל
        // ברגע שההיפנוזה נגמרת (isActionCompleted חוזר ל-False), הקוד הזה ירוץ שוב
        
        float distanceToPlayer = 999f;
        if (playerTarget != null)
        {
            distanceToPlayer = Vector3.Distance(transform.position, playerTarget.position);
        }

        if (distanceToPlayer <= detectionRange)
        {
            HandleAggro(distanceToPlayer);
        }
        else
        {
            HandlePatrol();
        }
    }

    private void HandleAggro(float distance)
    {
        isAggro = true;
        FaceTarget(playerTarget.position);

        if (attackType == AttackType.RangedShooter)
        {
            if (spawnScript != null) spawnScript.enabled = true;
            SetAnimBool("IsAttacking", true);
            SetAnimBool("IsChasing", false);
        }
        else if (attackType == AttackType.MeleeChaser)
        {
            float step = chaseSpeed * Time.deltaTime;
            Vector3 targetPos = new Vector3(playerTarget.position.x, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

            SetAnimBool("IsChasing", true);
            SetAnimBool("IsAttacking", false);
        }
    }

    private void HandlePatrol()
    {
        if (isAggro)
        {
            isAggro = false;
            if (spawnScript != null) spawnScript.enabled = false;
            SetAnimBool("IsAttacking", false);
            SetAnimBool("IsChasing", false);
            
            // איפוס קטן כדי שהפטרול ימשיך חלק
            horizontalOffset = transform.position.x - startPosition.x;
        }

        horizontalOffset += direction * moveSpeed * Time.deltaTime;
        transform.localScale = new Vector3(-direction, 1, 1); 

        if (Mathf.Abs(horizontalOffset) >= patrolDistance)
        {
            direction *= -1;
            horizontalOffset = Mathf.Clamp(horizontalOffset, -patrolDistance, patrolDistance);
        }

        float verticalOffset = 0f;
        if (useFloating)
        {
            verticalOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        }
        transform.position = new Vector3(startPosition.x + horizontalOffset, startPosition.y + verticalOffset, startPosition.z);
    }

    private void SetAnimBool(string paramName, bool value)
    {
        if (animator != null && !string.IsNullOrEmpty(paramName))
            animator.SetBool(paramName, value);
    }

    private void FaceTarget(Vector3 target)
    {
        if (target.x > transform.position.x)
            transform.localScale = new Vector3(-1, 1, 1);
        else
            transform.localScale = new Vector3(1, 1, 1);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.cyan;
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        Gizmos.DrawLine(center + Vector3.left * patrolDistance, center + Vector3.right * patrolDistance);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}