using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct HazardType
{
    public string tag;      
    public int damage;      
    public Color hitColor;
    
    [Header("Behavior Settings")]
    public bool respawnPlayer;   // TRUE = Teleport to start. FALSE = Just damage.
    public bool isContinuous;    // TRUE = Damage repeats (Fire). FALSE = One hit (Spike).
    public float damageInterval; // Time between hits (e.g., 1.0s)
    
    public AudioClip hitSound;   
}

public class PlayerDetect : MonoBehaviour
{
    [Header("Game References")]
    public GameObject playerRoot; 
    public Transform StartPoint;
    public Health health;
    public TMPro.TMP_Text numberAppleText;
    
    [Header("Audio")]
    public AudioSource collectSound;
    public AudioSource trophySound;
    public AudioSource failSound; 

    [Header("Collection Settings")]
    public int countApple = 0;
    public UnityEvent OnTrophyCollected;

    [Header("Enemy & Hazards Settings")]
    public List<HazardType> hazards = new List<HazardType>(); 
    
    [Header("Fail Feedback Settings")]
    public float failDelay = 1f; 
    public float flashDuration = 0.2f; 
    public GameObject failMessageUI; 
    
    private SpriteRenderer playerSprite;
    private Rigidbody2D playerRb;
    private SimplePlayer movementScript;
    
    private bool isDead = false; 
    
    // --- NEW: Timer for continuous damage ---
    private float nextDamageTime = 0f; 

    private void Start()
    {
        movementScript = GetComponentInParent<SimplePlayer>();

        if (movementScript != null)
        {
            playerRb = movementScript.GetComponent<Rigidbody2D>();
            playerSprite = movementScript.GetComponentInChildren<SpriteRenderer>();
            playerRoot = movementScript.gameObject;
        }
        else
        {
            playerRb = GetComponentInParent<Rigidbody2D>();
            playerSprite = GetComponentInChildren<SpriteRenderer>();
            playerRoot = transform.parent != null ? transform.parent.gameObject : gameObject;
        }

        if (playerRb == null) Debug.LogError("PlayerDetect: Could not find Rigidbody2D on Parent!");
    }

    private void Update()
    {
        if (numberAppleText != null)
        {
            numberAppleText.text = countApple.ToString();
        }
    }

    // --- 1. ENTER TRIGGER (Items + One Time Hits) ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return; 

        // Apple Collection
        if (collision.CompareTag("apple"))
        {
            if(health) health.HealHP(20);
            collision.gameObject.SetActive(false);
            if(collectSound) collectSound.Play();
            countApple++;
            return;
        }

        // Trophy Collection
        if (collision.CompareTag("trophy"))
        {
            collision.gameObject.SetActive(false);
            countApple += 5;
            if(trophySound) trophySound.Play();
            if (OnTrophyCollected != null) OnTrophyCollected.Invoke();
            return;
        }

        // Check for ONE-TIME Hazards (Spikes / Bullets)
        foreach (HazardType hazard in hazards)
        {
            if (collision.CompareTag(hazard.tag))
            {
                // If it's NOT continuous, hit immediately
                if (!hazard.isContinuous)
                {
                    StartCoroutine(HandleOneTimeHit(hazard));
                }
                return; 
            }
        }
    }

    // --- 2. STAY TRIGGER (Continuous Damage Logic) ---
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (isDead) return;

        foreach (HazardType hazard in hazards)
        {
            // Only check hazards that are marked as CONTINUOUS (e.g., Fire, Poison)
            if (collision.CompareTag(hazard.tag) && hazard.isContinuous)
            {
                // Check if enough time has passed since last hit
                if (Time.time >= nextDamageTime)
                {
                    // Deal damage and reset timer
                    StartCoroutine(ApplyContinuousTick(hazard));
                    nextDamageTime = Time.time + hazard.damageInterval;
                }
            }
        }
    }

    // --- 3. EXIT TRIGGER (Cleanup) ---
    private void OnTriggerExit2D(Collider2D collision)
    {
        // Just ensure color is reset when leaving any hazard
        foreach (HazardType hazard in hazards)
        {
            if (collision.CompareTag(hazard.tag))
            {
                if (playerSprite != null) playerSprite.color = Color.white;
            }
        }
    }

    // --- LOGIC: ONE TIME HIT ---
    private IEnumerator HandleOneTimeHit(HazardType hazardData)
    {
        isDead = true; 

        PlayHazardSound(hazardData);

        Color originalColor = Color.white;
        if (playerSprite != null)
        {
            originalColor = playerSprite.color;
            playerSprite.color = hazardData.hitColor; 
        }

        if (health != null) health.DamgerHP(hazardData.damage);

        if (hazardData.respawnPlayer)
        {
            // Respawn Sequence
            if (movementScript != null) movementScript.canMove = false;
            if (playerRb != null) 
            {
                playerRb.linearVelocity = Vector2.zero; 
                playerRb.simulated = false; 
            }

            if (failMessageUI != null) failMessageUI.SetActive(true);
            yield return new WaitForSeconds(failDelay);

            if (playerRoot != null && StartPoint != null)
                playerRoot.transform.position = StartPoint.position;

            if (failMessageUI != null) failMessageUI.SetActive(false);
            if (playerRb != null) playerRb.simulated = true; 
            if (movementScript != null) movementScript.canMove = true;
        }
        else
        {
            // Just Damage
            yield return new WaitForSeconds(flashDuration);
        }

        if (playerSprite != null) playerSprite.color = originalColor;
        isDead = false; 
    }

    // --- LOGIC: CONTINUOUS TICK (Single damage burst) ---
    private IEnumerator ApplyContinuousTick(HazardType hazardData)
    {
        // 1. Deal Damage
        if (health != null) health.DamgerHP(hazardData.damage);
        
        // 2. Play Sound
        PlayHazardSound(hazardData);

        // 3. Flash Color ON
        if (playerSprite != null) playerSprite.color = hazardData.hitColor;

        // 4. Wait briefly (flash length)
        yield return new WaitForSeconds(0.2f); 

        // 5. Flash Color OFF
        if (playerSprite != null) playerSprite.color = Color.white;
    }

    private void PlayHazardSound(HazardType data)
    {
        if (data.hitSound != null) failSound.PlayOneShot(data.hitSound); 
        else if (failSound != null) failSound.Play();
    }
}