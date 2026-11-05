using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    [Header("Settings")]
    public bool openOnStart = true; // Set to true if you want the menu to open automatically when the game starts

    [Header("Reference")]
    public GameObject MenuCanvas;
public GameObject pauseUi;
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

    public void OnRestartPress()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnGameResumePress()
    {
 pauseUi.SetActive(false);
    }

    public void OnEnterPausePress()
    {
        pauseUi.SetActive(true);
    }

    public void OnGameExitPress()
    {
Application.Quit();
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

