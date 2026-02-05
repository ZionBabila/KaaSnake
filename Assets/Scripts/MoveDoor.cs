using System.Net;
using UnityEngine;

public class MoveDoor : MonoBehaviour
{
    public Transform endPoint;
    public PlayerDetect playerDetect;

    private void Update()
    {
        if (playerDetect.countApple >= 8)
        {
            transform.position = Vector2.Lerp(transform.position, endPoint.position, Time.deltaTime);
        }
    }
}
