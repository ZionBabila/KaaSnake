using System;
using UnityEditor.VisionOS;
using UnityEngine;
using UnityEngine.UI;

public class CharacterOption : MonoBehaviour
{
    int id;
    CharacterManager characterManager;
    public Image characterImage;
    public void OnClick()
    {
        characterManager.CharacterClicked(id);
        Debug.Log(Convert.ToString(id));

    }
    public void Initialize(CharacterManager characterManager, int id)
    {
        this.characterManager = characterManager;
        this.id = id;
        characterImage.sprite = characterManager.characterSprites[id];
    }
}



