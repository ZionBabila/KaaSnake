using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Health : MonoBehaviour

{
    [Header("UI Elements")]
    public Slider PlayerHPslider;
    public GameObject FailTitle;
    [Header("Health Settings")]
    public int CurrentHP = 100;
    public int MaxHP = 100;
    public bool isDead = false;
    [Header("Reference & Audio")]
    public SimplePlayer simplePlayer;
    public AudioSource heat;
    private void Start()
    {
        PlayerHPslider.maxValue = MaxHP;
        FailTitle.SetActive(false);

    }
    public void DamgerHP(int damage)
    {
        if (isDead)
        {
            return;
        }
        if (heat != null)
        {
            heat.Play();
        }
        CurrentHP -= damage;
        if (CurrentHP < 0)
        {
            CurrentHP = 0;
            StartCoroutine(OnDead());
            isDead = true;
        }
        PlayerHPslider.value = CurrentHP;

    }
    public void HealHP(int heal)
    {
        CurrentHP += heal;
        if (CurrentHP > MaxHP)
        {
            CurrentHP = MaxHP;
        }
        PlayerHPslider.value = CurrentHP;
    }

    private IEnumerator OnDead()
    {
        FailTitle.SetActive(true);
        simplePlayer.enabled = false;
        yield return new WaitForSeconds(1.5f);
        // get the current scene name and reload it
        string currentSceneName = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentSceneName);
        yield break;

    }
}
