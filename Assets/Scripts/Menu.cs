using UnityEngine;

public class Menu : MonoBehaviour
{
    [Header("Settings")]
    public bool openOnStart = true; // Set to true if you want the menu to open automatically when the game starts

    [Header("Reference")]
    public GameObject MenuCanvas;

    private void Start()
    {
        if (openOnStart)
        {
            OpenMenu();
        }
        else
        {
            CloseMenu();
        }
    }
    public void OpenMenu()
    {
        if (MenuCanvas != null)
        {
            MenuCanvas.SetActive(true);
            Time.timeScale = 0f; // Pause the game           
        }

    }

    public void CloseMenu()
    {
        if (MenuCanvas != null)
        {
            MenuCanvas.SetActive(false);
            Time.timeScale = 1f;
        }

    }
    public void OnQuit()
    {
        Application.Quit();
    }
}

