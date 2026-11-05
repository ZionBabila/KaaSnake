using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AnnoyingButton : MonoBehaviour
{
    public float speed = 0.5f;
    public LeanTweenType leanTweenType;
    public void ButtonClicked()
    {
        Vector3 newPosition = new Vector3(Random.Range(-200, 200), Random.Range(-200, 200), 0);
        transform.LeanMoveLocal(newPosition, speed).setEase(leanTweenType);
    }
}
