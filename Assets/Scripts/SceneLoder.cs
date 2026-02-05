using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoder : MonoBehaviour
{
    public string SceneName;
    public GameObject NextLevelTitel;
    public bool StartLoad = false;
    public Image fadeImage;
    private void Start()
    {
        fadeImage.canvasRenderer.SetAlpha(0.0f);
        if (NextLevelTitel != null)
        {
            NextLevelTitel.SetActive(false);
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !StartLoad)
        {

            LoadScene();
            StartLoad = true;
        }
    }
    public void LoadScene()
    {


        StartCoroutine(OnNextLevel());

    }
    private IEnumerator OnNextLevel()
    {

        Debug.Log("coroutine started");
        NextLevelTitel.SetActive(true);
        fadeImage.canvasRenderer.SetAlpha(0.01f);
        fadeImage.CrossFadeAlpha(1f, 2.0f, true);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(SceneName);
        fadeImage.canvasRenderer.SetAlpha(1f);
        fadeImage.CrossFadeAlpha(0f, 2.0f, true);

        yield break;
    }
}

