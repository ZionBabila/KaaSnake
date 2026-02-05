//using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class MoveObstacles : MonoBehaviour
{
    public Transform startPoint;
    public Transform endPoint;
    public float Speed = 2f;
    private bool movingToEnd = true;
    public GameObject Player;



    void Update()
    {
        TrackPlayer();
    }


    private void TrackPlayer()
    {
        if (Player != null)
        {
            Vector3 pos = new Vector3(Player.transform.position.x, transform.position.y, transform.position.z);
            Vector3 dir = pos - transform.position;
            transform.position = transform.position + (dir.normalized * Speed * Time.deltaTime);
        }
    }
    private void PingPong()
    {
        if (movingToEnd == true)
        {
            transform.position = Vector2.Lerp(transform.position, endPoint.position, Time.deltaTime);
            float distance = Vector3.Distance(transform.position, endPoint.position);
            if (Vector2.Distance(transform.position, endPoint.position) < 0.1f)
            {
                movingToEnd = false;
            }
        }
        else
        {
            transform.position = Vector2.Lerp(transform.position, startPoint.position, Time.deltaTime);
            float distance = Vector3.Distance(transform.position, startPoint.position);
            if (Vector2.Distance(transform.position, startPoint.position) < 0.1f)
            {
                movingToEnd = true;
            }
        }
    }

}
