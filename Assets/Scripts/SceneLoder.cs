using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Level Settings")]
    [Tooltip("The exact name of the next scene to load.")]
    public string SceneName; 
    
    [Tooltip("UI Object (Text/Image) that says 'Level Complete' or similar.")]
    public GameObject NextLevelTitle; 
    
    [Tooltip("Black Image used for fading the screen out.")]
    public Image fadeImage;

    [Header("Audio")]
    [Tooltip("Sound to play immediately when the level ends (Win sound).")]
    public AudioSource finishSound; 

    // Internal flag to prevent triggering the win multiple times
    private bool startLoad = false;

    private void Start()
    {
        // Ensure the fade image is transparent at the start
        if (fadeImage != null)
        {
            fadeImage.canvasRenderer.SetAlpha(0.0f);
        }

        // Hide the "Level Complete" text at the start
        if (NextLevelTitle != null)
        {
            NextLevelTitle.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object colliding is the Player AND if we haven't already won
        if (other.CompareTag("Player") && !startLoad)
        {
            startLoad = true; // Lock the logic so it happens only once

            // 1. Immediately freeze the player (Physics & Controls)
            FreezePlayer(other.gameObject);

            // 2. Play the Victory Sound
            if (finishSound != null)
            {
                finishSound.Play();
            }

            // 3. Start the visual transition sequence
            StartCoroutine(OnNextLevel());
        }
    }

    // This function stops the player completely
    private void FreezePlayer(GameObject player)
    {
        // A. Disable the movement script (so keyboard input stops working)
        SimplePlayer movement = player.GetComponent<SimplePlayer>();
        if (movement != null)
        {
            movement.enabled = false;
        }

        // B. Stop Physics (so the player doesn't slide or fall)
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Stop current movement
            rb.linearVelocity = Vector2.zero; 
            // rb.velocity = Vector2.zero; // Use this if you are on an older Unity version
            
            // Disable physics simulation (Freezes player in place)
            rb.simulated = false; 
        }

        // C. (Optional) Stop running animation
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            // Assuming you have a Speed parameter, set it to 0
            anim.SetFloat("Speed", 0); 
            // anim.SetTrigger("Victory"); // If you have a victory dance, trigger it here!
        }
    }

    private IEnumerator OnNextLevel()
    {
        Debug.Log("Level Finished! Starting transition...");

        // Show "Level Complete" Text
        if (NextLevelTitle != null)
        {
            NextLevelTitle.SetActive(true);
        }

        // Start fading the screen to black
        if (fadeImage != null)
        {
            fadeImage.canvasRenderer.SetAlpha(0.0f);
            // Fades alpha from 0 to 1 over 1.5 seconds
            fadeImage.CrossFadeAlpha(1f, 1.5f, true); 
        }

        // Wait for 2 seconds (Let the player hear the music and see the text)
        yield return new WaitForSeconds(2f);

        // Load the next scene
        SceneManager.LoadScene(SceneName);
    }
}