using UnityEngine;
using TMPro;
using UnityEngine.UI; // Required for the Slider
using System.Collections;

public class PlayerHypnotize : MonoBehaviour
{
    // Global variable accessible from any script
    public static bool IsHypnotizing { get; private set; }
    
    private Animator playerAnim;
    private SimplePlayer playerMovement;

    [Header("Detection Settings")]
    public float agroRange = 10f;
    public Transform playerCastPoint;
    public LayerMask detectionLayer;

    [Header("Hypnosis Power (Mana)")]
    public float currentPower = 0f;      // Current available power
    public float maxPower = 100f;        // Maximum power limit
    public float powerCostPerUse = 50f;  // Cost for one full hypnosis action

    [Header("UI References")]
    public TextMeshProUGUI promptText;   // Text for messages (e.g., "Hold Space...")
    public Slider powerSlider;           // Reference to the UI Slider

    [Header("Audio Settings")]
    public AudioSource hypnotizeSound;
    [Range(0.1f, 3.0f)] public float successFadeDuration = 1.5f; 
    [Range(0.1f, 1.0f)] public float failFadeDuration = 0.2f;

    private float holdTimer = 0f;
    private bool isWaitingForEnemy = false;
    private HypnotizableEntity currentTarget;
    private Coroutine fadeCoroutine;

    void Awake()
    {
        playerAnim = GetComponentInParent<Animator>();
        playerMovement = GetComponent<SimplePlayer>();
    }

    void Start()
    {
        // Initialize the slider at the start of the game
        if (powerSlider != null)
        {
            powerSlider.maxValue = maxPower;
            powerSlider.value = currentPower;
            Debug.Log($"[Hypno] Slider Initialized. Max: {maxPower}, Current: {currentPower}");
        }
        else
        {
            Debug.LogWarning("[Hypno] Power Slider is NOT assigned in the Inspector!");
        }
    }

    // --- Function to add power (Called by Golden Apple) ---
    public void AddHypnoPower(float amount)
    {
        Debug.Log($"[Hypno] AddHypnoPower called. Adding: {amount}. Current before: {currentPower}");
        
        currentPower += amount;
        
        // Clamp the power so it doesn't exceed the maximum
        if (currentPower > maxPower) currentPower = maxPower;

        UpdatePowerUI(); // Update the visual slider

        // Show a temporary message on screen
        if(promptText != null) 
        {
            promptText.gameObject.SetActive(true);
            promptText.text = "Power Up!";
            Invoke("ClearPrompt", 2f);
        }
        
        Debug.Log($"[Hypno] Power Updated. New Value: {currentPower}");
    }
    
    private void ClearPrompt() 
    { 
        if(promptText) promptText.gameObject.SetActive(false); 
    }

    private void UpdatePowerUI()
    {
        if (powerSlider != null)
        {
            powerSlider.value = currentPower;
            Debug.Log($"[Hypno] Slider UI updated to: {currentPower}");
        }
        else
        {
            Debug.LogWarning("[Hypno] Cannot update UI - Slider reference is missing!");
        }
    }

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
            // If the enemy is already hypnotized, do nothing
            if (currentTarget.isActionCompleted)
            {
                IsHypnotizing = false; 
                return;
            }

            // --- Check: Is there enough power? ---
            if (currentPower < powerCostPerUse)
            {
                // If player presses the button without enough power, show error
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton0))
                {
                    promptText.gameObject.SetActive(true);
                    promptText.text = "Not enough Power!";
                    // Optional: Add an error sound here
                }
                IsHypnotizing = false;
                return; 
            }
            // --------------------------------

            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.JoystickButton0))
            {
                ExecuteHypnosis();
            }
            else
            {
                // Released button mid-action
                if (holdTimer > 0) StartFade(failFadeDuration);
                
                IsHypnotizing = false;
                promptText.gameObject.SetActive(true);
                promptText.text = "Hold SPACE to Hypnotize";
                ResetHypnosisState(); 
            }
        }
        else
        {
            // Looking away from enemy
            if (hypnotizeSound.isPlaying && fadeCoroutine == null)
            {
                StartFade(failFadeDuration);
            }
            IsHypnotizing = false;
            ClearAll();
        }

        if(playerAnim != null)
        {
            playerAnim.SetBool("isHypnotizing", IsHypnotizing);
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
            IsHypnotizing = false; 
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
            // Clear text only if it's NOT showing the "Power Up" message
            if (promptText.text != "Power Up!")
            {
                promptText.text = "";
                promptText.gameObject.SetActive(false);
            }
        }
    }

    private void FinishSuccess()
    {
        isWaitingForEnemy = false;
        holdTimer = 0f;
        
        // --- Reduce power after success ---
        currentPower -= powerCostPerUse;
        if (currentPower < 0) currentPower = 0;
        
        UpdatePowerUI(); // Update the slider visual
        Debug.Log($"[Hypno] Hypnosis Successful. Power reduced. Current: {currentPower}");
        // ---------------------------

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

    private float lastFacingDir = 1f;

    private bool VisualizeAndCheckLine()
    {
        if (playerCastPoint == null) return false;

        if (playerMovement == null)
        {
            playerMovement = GetComponentInParent<SimplePlayer>();
            if (playerMovement == null)
            {
                Debug.LogError("[Hypno] SimplePlayer script not found on Parent!");
                return false;
            }
        }

        if (playerMovement.V > 0.01f) lastFacingDir = 1f;
        else if (playerMovement.V < -0.01f) lastFacingDir = -1f;

        Vector2 direction = new Vector2(lastFacingDir, 0);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(playerCastPoint.position, direction, agroRange, detectionLayer);
        
        bool hitEnemy = false;
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.transform.root == transform.root) continue;

            if (hit.collider.CompareTag("enemy_eye"))
            {
                currentTarget = hit.collider.GetComponentInParent<HypnotizableEntity>();
                hitEnemy = true;
                break; 
            }
        }
        
        return hitEnemy;
    }

    private void OnDrawGizmos()
    {
        if (playerCastPoint == null) return;

        float dir = (Application.isPlaying) ? lastFacingDir : 1f;
        Vector3 direction = new Vector3(dir, 0, 0);

        Gizmos.color = Color.yellow;
        Vector3 startPos = playerCastPoint.position;
        Vector3 endPos = startPos + (direction * agroRange);
        Gizmos.DrawLine(startPos, endPos);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(endPos, 0.2f);
    }
}