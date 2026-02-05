using UnityEngine;
using UnityEngine.Events;

public class HypnotizableEntity : MonoBehaviour
{
    [Header("Hypnosis Stats")]
    public string entityName = "Enemy";
    public float requiredTime = 2.0f; // Seconds needed to hypnotize this specific animal
    public bool isActionCompleted = false;

    [Header("Animator Parameters")]
    public string tryingBool = "isHypnoProcess";    // Animation while player holds Space
    public string completedBool = "isFullyHypnotized"; // Final state after 100%

    [Header("Events")]
    public UnityEvent OnHypnosisSuccess; // Trigger world events (e.g., open door)

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void UpdateHypnosisProgress(float progressPercent)
    {
        // Don't update if already finished
        if (isActionCompleted) return;

        if (progressPercent >= 100f)
        {
            FinishHypnosis();
        }
        else if (progressPercent > 0)
        {
            SetAnimatorStates(true, false);
        }
        else
        {
            SetAnimatorStates(false, false);
        }
    }

    private void FinishHypnosis()
    {
        isActionCompleted = true;
        SetAnimatorStates(false, true); // Stop "trying", start "complete"
        
        if (OnHypnosisSuccess != null)
            OnHypnosisSuccess.Invoke();

        Debug.Log($"{entityName} is fully hypnotized!");
    }

    private void SetAnimatorStates(bool trying, bool completed)
    {
        if (anim == null) return;
        anim.SetBool(tryingBool, trying);
        anim.SetBool(completedBool, completed);
    }

    private void OnDrawGizmos()
    {
        // Draw a visual marker above the animal in the editor
        Gizmos.color = isActionCompleted ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.3f);
    }
}