using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ScrollTools : MonoBehaviour
{
    public GameObject goToTopButton;
    public ScrollRect scrollRect;

    public void OnEnable()
    {
        scrollRect.verticalNormalizedPosition = 1;
    }
    public void OnScrollChanged (Vector2  newValues)
    {
        goToTopButton.SetActive(newValues.y < 0.8);
       
    }
    public void OnGoToTop()
    {
        scrollRect.verticalNormalizedPosition = 1;
    }
}