using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoder : MonoBehaviour
{
    public string SceneName;
    public GameObject NextLevelTitel;
    public bool StartLoad = false;

private void Start()
    {
     NextLevelTitel.SetActive(false);   
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter2D(Collider2D other)
    {
        LateUpdate();
    }
    public void LateUpdate()
    {
       StartCoroutine(OnNextLevel());
    }
    private IEnumerator OnNextLevel()
    {
        Debug.Log("coroutine started");
        NextLevelTitel.SetActive(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(SceneName);
        yield break;
    }
}

