using UnityEngine;

public class SpanwObject : MonoBehaviour
{
    public int damage = 20;
    public AudioSource hitSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // פגיעה בשחקן
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.DamgerHP(damage); // תיקנתי ל-DamageHP (שים לב שזה תואם לסקריפט ה-Health שלך)
                
                if (hitSound != null)
                {
                    // שימוש ב-PlayClipAtPoint כדי שהסאונד יישמע גם אם האובייקט מושמד
                    AudioSource.PlayClipAtPoint(hitSound.clip, transform.position);
                }
            }
            Destroy(gameObject);
        }
        
        // פגיעה ברצפה
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}