using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player; 
    public float offsetY = 5f;

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 pos = transform.position;
        pos.x = player.position.x;
        pos.y = player.position.y + offsetY;
        pos.z = -10f;   // always fixed for 2D camera
        transform.position = pos;
    }
}
