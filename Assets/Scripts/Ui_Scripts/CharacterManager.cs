using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.TextCore.Text;
using System;
using UnityEditor.Embree;

public class CharacterManager : MonoBehaviour
{
    public Image previewImage;
    public Sprite[] characterSprites;
    public GameObject characterOptionPrefab;
    public Transform characterContainer;
    public Color[] colors;
    private void Start()
    {
        foreach (Transform item in characterContainer)
        {
            Destroy(item.gameObject);
        }
        CharacterClicked(0);
        SetupCharacters();
        // ChangeColor(0);
        ChangeColor(colors[0]);
    }
    public void CharacterClicked(int id)
    {
        previewImage.sprite = characterSprites[id];
        previewImage.SetNativeSize();
    }
    void SetupCharacters()
    {
        for (int i = 0; i < characterSprites.Length; i++)
        {
            GameObject go = Instantiate(characterOptionPrefab, characterContainer);
            CharacterOption characterOption = go.GetComponent<CharacterOption>();
            characterOption.Initialize(this, i);
        }

    }
    public void ChangeColor(int option)
    {
        previewImage.color = colors[option];
    }

    public void ChangeColor(Color color)
    {
        previewImage.color = color;
    }
}
