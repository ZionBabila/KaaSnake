using UnityEngine;

public class SimplePatrol : MonoBehaviour
{
    [Header("Horizontal Movement")]
    public float moveSpeed = 2f;
    public float patrolDistance = 3f;
    public bool moveRightFirst = true;

    [Header("Vertical Floating (Subtle)")]
    public bool useFloating = true;
    public float floatAmplitude = 0.15f; // Very gentle bobbing
    public float floatSpeed = 2.5f;

    [Header("References")]
    public HypnotizableEntity hypnosisSystem;
    public bool showGizmos = true;

    private Vector3 startPosition;
    private float horizontalOffset = 0f;
    private int direction = 1;

    void Start()
    {
        startPosition = transform.position;
        direction = moveRightFirst ? 1 : -1;

        if (hypnosisSystem == null) 
            hypnosisSystem = GetComponent<HypnotizableEntity>();
    }

    void Update()
    {
        // Stop the horizontal patrol if hypnosis is finished
        if (hypnosisSystem != null && hypnosisSystem.isActionCompleted) return;

        HandlePatrol();
    }

    private void HandlePatrol()
    {
        // 1. Horizontal logic
        horizontalOffset += direction * moveSpeed * Time.deltaTime;
        if (Mathf.Abs(horizontalOffset) >= patrolDistance)
        {
            direction *= -1;
            horizontalOffset = Mathf.Clamp(horizontalOffset, -patrolDistance, patrolDistance);
        }

        // 2. Vertical bobbing logic (Sine wave)
        float verticalOffset = 0f;
        if (useFloating)
        {
            verticalOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        }

        // 3. Apply position
        transform.position = new Vector3(startPosition.x + horizontalOffset, startPosition.y + verticalOffset, startPosition.z);
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.cyan;
        Vector3 center = Application.isPlaying ? startPosition : transform.position;
        Vector3 left = center + Vector3.left * patrolDistance;
        Vector3 right = center + Vector3.right * patrolDistance;

        Gizmos.DrawLine(left, right);
        Gizmos.DrawWireCube(left, new Vector3(0.1f, 0.4f, 0.1f));
        Gizmos.DrawWireCube(right, new Vector3(0.1f, 0.4f, 0.1f));
    }
}