using UnityEngine;
using System.Collections;

public class WorldTrigger : MonoBehaviour
{
    [Header("General Settings")]
    public float delay = 0.5f;
    public float actionSpeed = 2.0f;

    [Header("New: Exit Logic (Optional)")]
    // Drag the collider that triggers the level transition here
    public Collider2D exitCollider; 

    [Header("Visual Swap (Optional)")]
    public SpriteRenderer targetRenderer;
    public Sprite newSprite;

    [Header("Rotation (Optional)")]
    public bool useRotation = false;
    public Vector3 targetRotation;

    [Header("Movement (Optional)")]
    public bool usePosition = false;
    public Vector3 targetPosition;

    [Header("Animator (Optional)")]
    public Animator targetAnimator; 
    public string triggerName = "open";

    private bool isActivated = false;

    private void Awake()
    {
        // Ensure the exit collider is disabled at the start of the level
        if (exitCollider != null)
        {
            exitCollider.enabled = false;
        }
    }

    public void OpenPath()
    {
        if (isActivated) return;
        isActivated = true;

        StartCoroutine(ExecuteAction());
    }

    private IEnumerator ExecuteAction()
    {
        // 1. Wait for the initial delay
        yield return new WaitForSeconds(delay);

        // 2. Activate Animator Trigger
        if (targetAnimator != null && !string.IsNullOrEmpty(triggerName))
        {
            targetAnimator.SetTrigger(triggerName);
        }

        // 3. Swap Sprite
        if (targetRenderer != null && newSprite != null)
        {
            targetRenderer.sprite = newSprite;
        }

        // 4. Movement and Rotation Logic
        if (useRotation || usePosition)
        {
            float t = 0;
            Vector3 startPos = transform.localPosition;
            Quaternion startRot = transform.localRotation;
            Quaternion targetRot = Quaternion.Euler(targetRotation);

            while (t < 1f)
            {
                t += Time.deltaTime * actionSpeed;

                if (useRotation)
                    transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
                
                if (usePosition)
                    transform.localPosition = Vector3.Lerp(startPos, targetPosition, t);

                yield return null;
            }

            if (useRotation) transform.localRotation = targetRot;
            if (usePosition) transform.localPosition = targetPosition;
        }

        // 5. New: Enable the transition collider after movement is done
        if (exitCollider != null)
        {
            exitCollider.enabled = true;
            Debug.Log(gameObject.name + ": Exit Collider Enabled!");
        }

        Debug.Log(gameObject.name + " action completed.");
    }
}