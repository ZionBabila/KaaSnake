using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Health : MonoBehaviour
{
    public Slider PlayerHPslider;
    public int CurrentHP = 100;
    public int MaxHP = 100;
    public bool isDead = false;
    public GameObject FailTitle;
    public SimplePlayer simplePlayer;
    public string sceneName;
    public AudioSource heat;
    private void Start()
    {
        PlayerHPslider.maxValue = MaxHP;
        FailTitle.SetActive(false);
        
    }
    public void DamgerHP(int damage)
    {
        if(heat != null)
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
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("zion 2");
        yield break;

    }
}
