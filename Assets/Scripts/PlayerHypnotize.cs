using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerHypnotize : MonoBehaviour
{
    // Global variable accessible from any script via PlayerHypnotize.IsHypnotizing
    public static bool IsHypnotizing { get; private set; }

    [Header("Detection Settings")]
    public float agroRange = 10f;
    public Transform playerCastPoint;
    public LayerMask detectionLayer;

    [Header("Audio Settings")]
    public AudioSource hypnotizeSound;
    [Range(0.1f, 3.0f)] public float successFadeDuration = 1.5f; 
    [Range(0.1f, 1.0f)] public float failFadeDuration = 0.2f;

    [Header("UI Reference")]
    public TextMeshProUGUI promptText;

    private float holdTimer = 0f;
    private bool isWaitingForEnemy = false;
    private HypnotizableEntity currentTarget;
    private Coroutine fadeCoroutine;

    void Update()
    {
        if (promptText == null) return;

        bool isLookingAtEnemy = VisualizeAndCheckLine();

        // 1. Success transition state
        if (isWaitingForEnemy)
        {
            if (currentTarget != null && currentTarget.isActionCompleted)
            {
                FinishSuccess();
            }
            return;
        }

        // 2. Main Logic
        if (isLookingAtEnemy && currentTarget != null)
        {
            if (currentTarget.isActionCompleted)
            {
                promptText.gameObject.SetActive(true);
                promptText.text = "Target is Hypnotized";
                IsHypnotizing = false; // Action is already done
                return;
            }

            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
            {
                ExecuteHypnosis();
            }
            else
            {
                // Released button
                if (holdTimer > 0) StartFade(failFadeDuration);
                
                IsHypnotizing = false;
                promptText.gameObject.SetActive(true);
                promptText.text = "Hold SPACE to Hypnotize";
                ResetHypnosisState(); 
            }
        }
        else
        {
            // Looking away
            if (hypnotizeSound.isPlaying && fadeCoroutine == null)
            {
                StartFade(failFadeDuration);
            }
            IsHypnotizing = false;
            ClearAll();
        }
    }

    private void ExecuteHypnosis()
    {
        IsHypnotizing = true;
        StopActiveFade();

        if (!hypnotizeSound.isPlaying)
        {
            hypnotizeSound.volume = 1f;
            hypnotizeSound.loop = true;
            hypnotizeSound.Play();
        }

        holdTimer += Time.deltaTime;
        float progress = (holdTimer / currentTarget.requiredTime) * 100f;
        
        promptText.gameObject.SetActive(true);
        promptText.text = $"Hypnotizing... {Mathf.Min(progress, 100f):0}%";
        
        currentTarget.UpdateHypnosisProgress(progress);

        if (holdTimer >= currentTarget.requiredTime)
        {
            isWaitingForEnemy = true;
            IsHypnotizing = false; // Transitioning to success state
            currentTarget.UpdateHypnosisProgress(100f); 
            StartFade(successFadeDuration);
        }
    }

    private void ResetHypnosisState()
    {
        holdTimer = 0f;
        if (currentTarget != null && !isWaitingForEnemy) 
        {
            currentTarget.UpdateHypnosisProgress(0);
        }
    }

    private void ClearAll()
    {
        if (!isWaitingForEnemy) 
        {
            if(currentTarget != null)
            {
                currentTarget.UpdateHypnosisProgress(0);
            }
            holdTimer = 0f;
            currentTarget = null;
        }
        
        if (promptText != null) 
        {
            promptText.text = "";
            promptText.gameObject.SetActive(false);
        }
    }

    private void FinishSuccess()
    {
        isWaitingForEnemy = false;
        holdTimer = 0f;
        
        if (promptText != null) 
        {
            promptText.gameObject.SetActive(true);
            promptText.text = "SUCCESS!";
        }
        currentTarget = null;
    }

    private void StartFade(float duration)
    {
        StopActiveFade();
        fadeCoroutine = StartCoroutine(FadeOutAudio(duration));
    }

    private void StopActiveFade()
    {
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
    }

    private IEnumerator FadeOutAudio(float duration)
    {
        float startVolume = hypnotizeSound.volume;
        float timer = 0;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            hypnotizeSound.volume = Mathf.Lerp(startVolume, 0, timer / duration);
            yield return null;
        }

        hypnotizeSound.Stop();
        hypnotizeSound.volume = 1f; 
        fadeCoroutine = null;
    }

    private bool VisualizeAndCheckLine()
    {
        if (playerCastPoint == null) return false;

        Vector2 endPos = (Vector2)playerCastPoint.position + (Vector2)playerCastPoint.right * agroRange;
        RaycastHit2D hit = Physics2D.Linecast(playerCastPoint.position, endPos, detectionLayer);
        
        bool hitEnemy = hit.collider != null && hit.collider.CompareTag("enemy_eye");

        if (hitEnemy)
        {
            currentTarget = hit.collider.GetComponentInParent<HypnotizableEntity>();
        }

        Debug.DrawLine(playerCastPoint.position, endPos, hitEnemy ? Color.green : Color.red);
        return hitEnemy;
    }
}