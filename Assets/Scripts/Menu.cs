using UnityEngine;

public class Menu : MonoBehaviour
{
public GameObject MenuCanvas;


private void Start()
    {
        OpenMenu();
    }
public void OpenMenu()
    {
        MenuCanvas.SetActive(true);
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        MenuCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
    public void OnQuit()
    {
        Application.Quit();
    }
}

