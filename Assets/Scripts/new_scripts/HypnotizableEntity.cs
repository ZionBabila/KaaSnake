using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class HypnotizableEntity : MonoBehaviour
{
    [Header("Hypnosis Stats")]
    public string entityName = "Enemy";
    public float requiredTime = 2.0f; 
    public bool isActionCompleted = false;

    [Header("Repeatable Settings")]
    public bool isRepeatable = true; 
    public float recoveryTime = 5.0f; 

    [Header("Animator Parameters")]
    public string tryingBool = "isHypnoProcess";    
    public string completedBool = "isFullyHypnotized"; 

    // --- New Settings for Tag and Layer Switching ---
    [Header("Neutral State Settings")]
    [Tooltip("If checked, the enemy's Tag and Layer will change when hypnotized so it won't hurt the player.")]
    public bool changeFactionOnHypnosis = false; // Default is false for safety
    public string neutralLayerName = "Default"; 
    public string neutralTagName = "Untagged";  
    
    // Variables to store the original state
    private string originalTag;
    private int originalLayer;

    [Header("Events")]
    public UnityEvent OnHypnosisSuccess; 
    public UnityEvent OnRecovery;        

    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
        
        // Save the original Tag and Layer at the start so we can restore them later
        originalTag = gameObject.tag;
        originalLayer = gameObject.layer;
    }

    public void UpdateHypnosisProgress(float progressPercent)
    {
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
        SetAnimatorStates(false, true); 
        
        // --- Change Tag and Layer (Only if checked in Inspector) ---
        if (changeFactionOnHypnosis)
        {
            SetNeutralState(true);
        }

        if (OnHypnosisSuccess != null)
            OnHypnosisSuccess.Invoke();

        Debug.Log($"{entityName} is fully hypnotized!");

        if (isRepeatable)
        {
            StartCoroutine(RecoverRoutine());
        }
    }

    private IEnumerator RecoverRoutine()
    {
        yield return new WaitForSeconds(recoveryTime);

        isActionCompleted = false; 
        SetAnimatorStates(false, false); 

        // --- Restore original Tag and Layer (Only if checked) ---
        if (changeFactionOnHypnosis)
        {
            SetNeutralState(false);
        }

        if (OnRecovery != null)
            OnRecovery.Invoke();

        Debug.Log($"{entityName} recovered from hypnosis.");
    }

    // Helper function to perform the switch
    private void SetNeutralState(bool isNeutral)
    {
        if (isNeutral)
        {
            // Switch to Neutral/Safe Mode
            gameObject.tag = neutralTagName;
            
            int layerIndex = LayerMask.NameToLayer(neutralLayerName);
            if (layerIndex != -1) 
                gameObject.layer = layerIndex;
            else
                Debug.LogWarning($"Layer '{neutralLayerName}' does not exist! Check spelling.");
        }
        else
        {
            // Switch back to Enemy Mode (Restore originals)
            gameObject.tag = originalTag;
            gameObject.layer = originalLayer;
        }
    }

    private void SetAnimatorStates(bool trying, bool completed)
    {
        if (anim == null) return;
        anim.SetBool(tryingBool, trying);
        anim.SetBool(completedBool, completed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = isActionCompleted ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position + Vector3.up * 2f, 0.3f);
    }
}