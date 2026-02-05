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
            health.DamgerHP(40);
            player.transform.position = StartPoint.position;
            failSound.Play();
            failCount += 1;
        }

    }
}
