using UnityEngine;

public class MemoryTest : MonoBehaviour
{
public int countApple = 0;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("apple") == true)
        {
            collision.gameObject.SetActive(false);
            countApple = countApple + 1;
        }
    }
}
