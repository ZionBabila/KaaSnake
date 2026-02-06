using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Image = UnityEngine.UI.Image;

public class PlayerDetect : MonoBehaviour
{

    public int countApple = 0;
    public GameObject tree;
    public Transform StartPoint;
    public GameObject player;
    public AudioSource collectSound;
    public AudioSource trophySound;
    public AudioSource failSound;
    public int failCount = 0;
    public TMPro.TMP_Text numberAppleText;
    public UnityEvent OnTrophyCollected;
    public Health health;
    [Header("Fail Settings")]
    public float failDelay = 1f; // Time to wait before respawning after hitting a bush
    public GameObject failMessageUI; // UI element to show when the player fails
    private void Start()
    {
         
    }
    private void Update()
    {
        if (numberAppleText != null)
        {
            numberAppleText.text = countApple.ToString();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("apple") == true)
        {
            health.HealHP(20);
            collision.gameObject.SetActive(false);
            collectSound.Play();
            countApple = countApple + 1;
        }
        if (collision.CompareTag("trophy") == true)
        {
            collision.gameObject.SetActive(false);
            countApple = countApple + 5;
            trophySound.Play();
            if (OnTrophyCollected != null) OnTrophyCollected.Invoke();
        }

        if (collision.CompareTag("bush") == true)
        {
            StartCoroutine(HandleBushFail());
        }

    }
    private IEnumerator HandleBushFail()
{
    // 1. Show the fail message
    if (failMessageUI != null) failMessageUI.SetActive(true);

    // 2. Play the sound immediately
    if (failSound != null) failSound.Play();

    // 3. Wait for the specified delay
    yield return new WaitForSeconds(failDelay);

    // 4. Teleport the player and update stats
    player.transform.position = StartPoint.position;
    health.DamgerHP(40);
    failCount += 1;

    // 5. Hide the message again
    if (failMessageUI != null) failMessageUI.SetActive(false);
}
}
