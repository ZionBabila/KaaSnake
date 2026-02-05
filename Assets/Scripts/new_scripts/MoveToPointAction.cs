using UnityEngine;
using UnityEngine.Events;

public class MoveToPointAction : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform targetPoint;
    public float speed = 3f;
    public string moveAnimBool = "isMoving";

    [Header("Return Logic (Optional)")]
    public bool returnAfterAction = false; 
    public float waitAtTarget = 1.0f; 
    private Vector3 startPosition;
    private bool isReturning = false;

    [Header("Actions to do at Destination")]
    public UnityEvent OnReachedDestination;

    private bool shouldMove = false;
    private Animator anim;

    void Awake() 
    {
        anim = GetComponent<Animator>();
        startPosition = transform.position;
    }

    void Update()
    {
        if (!shouldMove || targetPoint == null) return;

        Vector3 destination = isReturning ? startPosition : targetPoint.position;
        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, destination) < 0.1f)
        {
            if (!isReturning) ReachedTarget();
            else ReachedHome();
        }
    }

    public void StartMoving()
    {
        isReturning = false;
        shouldMove = true;
        
        if (anim != null) 
        {
            anim.SetBool(moveAnimBool, true);
            // RESET: וודא שההיפנוזה מתאפסת כדי שהאנימטור לא יתקע
            anim.SetBool("isFullyHypnotized", false); 
        }
    }

    private void ReachedTarget()
    {
        shouldMove = false;
        
        // התיקון הקריטי: מכבים את האנימציה רק אם האובייקט לא אמור לחזור (כמו הקוף)
        if (!returnAfterAction && anim != null) 
        {
            anim.SetBool(moveAnimBool, false);
        }
        
        transform.position = targetPoint.position;

        if (OnReachedDestination != null) OnReachedDestination.Invoke();

        if (returnAfterAction)
        {
            Invoke("StartReturn", waitAtTarget);
        }
    }

    private void StartReturn()
    {
        isReturning = true;
        shouldMove = true;
        // מדליקים שוב ליתר ביטחון (אם waitAtTarget היה גדול מ-0)
        if (anim != null) anim.SetBool(moveAnimBool, true);
    }

    private void ReachedHome()
    {
        shouldMove = false;
        // כאן אנחנו תמיד מכבים את האנימציה כי התנועה נגמרה סופית
        if (anim != null) anim.SetBool(moveAnimBool, false);
        transform.position = startPosition;
        Debug.Log(gameObject.name + " returned home.");
    }

    private void OnDrawGizmos()
    {
        if (targetPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position, targetPoint.position);
            Gizmos.DrawWireSphere(targetPoint.position, 0.3f);
        }
    }
}