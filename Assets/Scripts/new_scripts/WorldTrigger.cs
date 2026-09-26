using UnityEngine;
using System.Collections;

public class WorldTrigger : MonoBehaviour
{
    [Header("General Settings")]
    public float delay = 0.5f;
    public float actionSpeed = 2.0f;

    [Header("Exit Logic (Turns ON)")]
    // הקוליידר הזה יידלק בסוף הפעולה (למשל טריגר שמעביר שלב)
    public Collider2D exitCollider; 

    [Header("Blocker Logic (Turns OFF)")]
    // *** חדש: גרור לכאן את הקיר הבלתי נראה שאתה רוצה להעלים ***
    public Collider2D blockingCollider; 

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
        // 1. מוודאים שהיציאה כבויה בהתחלה
        if (exitCollider != null)
        {
            exitCollider.enabled = false;
        }

        // 2. מוודאים שהחוסם (הקיר) דלוק בהתחלה
        if (blockingCollider != null)
        {
            blockingCollider.enabled = true;
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

        // 4. Disable Blocker (Make the invisible wall disappear)
        // *** כאן הקיר הבלתי נראה נעלם ***
        if (blockingCollider != null)
        {
            blockingCollider.enabled = false;
            Debug.Log("Blocking collider disabled!");
        }

        // 5. Movement and Rotation Logic
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

        // 6. Enable the exit transition collider
        if (exitCollider != null)
        {
            exitCollider.enabled = true;
            Debug.Log(gameObject.name + ": Exit Collider Enabled!");
        }

        Debug.Log(gameObject.name + " action completed.");
    }
}