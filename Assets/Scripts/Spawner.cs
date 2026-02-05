using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject spawnObject;
    public Transform Target;
    public float Interval = 2.0f;
    public float ShiftPosition = 2;
    private float timer = 0;
    
    [Header("Physics & Range")]
    public float Force = 5;
    public float maxDistace = 10;

    [Header("Debug & Visuals")]
    // This checkbox will let you turn Gizmos on and off in the Inspector
    public bool showGizmos = true; 

    private bool isWorking = true; 

    private void Update()
    {
        if (!isWorking) return;

        // Check distance to target
        if (Target != null && Vector3.Distance(Target.position, transform.position) < maxDistace || Target == null)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;
            }
            else
            {
                Spawn();
                timer = Interval;
            }
        }
    }

    private void Spawn()
    {
        GameObject go = Instantiate(spawnObject);
        go.transform.position = transform.position + new Vector3(Random.Range(-ShiftPosition, ShiftPosition), 0, 0);
        
        if (Target != null)
        {
            Rigidbody2D rb = go.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0;
                Vector3 dir = Target.position - go.transform.position;
                rb.AddForce(dir.normalized * Force, ForceMode2D.Impulse);
                go.transform.rotation = Quaternion.FromToRotation(Vector3.up, -dir);
            }
        }
    }

    public void SetSpawnerActive(bool state)
    {
        isWorking = state;
    }

    // --- Visual Helpers ---
    private void OnDrawGizmos()
    {
        // Only draw if the checkbox is ticked in the Inspector
        if (!showGizmos) return;

        Gizmos.color = isWorking ? Color.red : Color.gray;
        
        // Draw the max shooting range
        Gizmos.DrawWireSphere(transform.position, maxDistace);
        
        // Draw the spawn offset area (where arrows appear)
        Vector3 leftBound = transform.position + new Vector3(-ShiftPosition, 0.5f, 0);
        Vector3 rightBound = transform.position + new Vector3(ShiftPosition, 0.5f, 0);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(leftBound, rightBound);

        if (Target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, Target.position);
        }
    }
}