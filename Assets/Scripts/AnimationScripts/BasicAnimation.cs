using UnityEngine;

public class BasicAnimation : MonoBehaviour
{
public Transform image;
    void Start()
    {
        image.LeanMoveLocalX(200, 2f);
    }

 
}
