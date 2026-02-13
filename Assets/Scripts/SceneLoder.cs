using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [Header("Level Settings")]
    [Tooltip("If true, the script will load the next scene. If false, it will only fade to black.")]
    public bool loadNextScene = true; 

    [Tooltip("The exact name of the next scene to load.")]
    public string SceneName; 
    
    [Tooltip("UI Object (Text/Image) that says 'Level Complete' or similar.")]
    public GameObject NextLevelTitle; 
    
    [Tooltip("Black Image used for fading the screen out.")]
    public Image fadeImage;

    [Header("Audio")]
    [Tooltip("Sound to play immediately when the level ends (Win sound).")]
    public AudioSource finishSound; 

    private bool startLoad = false;

    private void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.canvasRenderer.SetAlpha(0.0f);
        }

        if (NextLevelTitle != null)
        {
            NextLevelTitle.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !startLoad)
        {
            startLoad = true; 
            FreezePlayer(other.gameObject);

            if (finishSound != null)
            {
                finishSound.Play();
            }

            StartCoroutine(OnLevelEndSequence());
        }
    }

    private void FreezePlayer(GameObject player)
    {
        // Use your player script reference here
        // SimplePlayer movement = player.GetComponent<SimplePlayer>();
        // if (movement != null) movement.enabled = false;

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero; 
            rb.simulated = false; 
        }

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetFloat("Speed", 0); 
        }
    }

    private IEnumerator OnLevelEndSequence()
    {
        Debug.Log("Level Sequence Started...");

        // 1. Show Level Complete Title
        if (NextLevelTitle != null)
        {
            NextLevelTitle.SetActive(true);
        }

        // 2. Fade to black
        if (fadeImage != null)
        {
            fadeImage.canvasRenderer.SetAlpha(0.0f);
            fadeImage.CrossFadeAlpha(1f, 1.5f, true); 
        }

        // 3. Wait for the fade and sound to be experienced
        yield return new WaitForSeconds(2f);

        // 4. Decision: Load next scene or just stay black
        if (loadNextScene && !string.IsNullOrEmpty(SceneName))
        {
            SceneManager.LoadScene(SceneName);
        }
        else
        {
            Debug.Log("Fade complete. Staying in current scene as requested.");
        }
    }
}