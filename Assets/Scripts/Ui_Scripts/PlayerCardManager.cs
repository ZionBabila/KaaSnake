using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEditor.SceneManagement;


public class PlayerCardManager : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text descriptionText;
    public TMP_Text ageText;
    public TMP_InputField descriptionInputField;
  public void  UpdateName(string newName)
    {
        nameText.text = newName;
    }
public void UpdateDescription(string newDescription)
    {
        descriptionText.text = newDescription;

    }
    public void UpdateAge(float newAge)
    {
        ageText.text = newAge.ToString() + " Years old";
    
    }
    public void SubmitDescription(string newDescription)
    {
        if(Keyboard.current.leftShiftKey.isPressed || Keyboard.current.rightShiftKey.isPressed)
        
        {
            descriptionInputField.ActivateInputField();
            descriptionInputField.caretPosition = descriptionInputField.text.Length;
            return;

        }
        descriptionText.text = newDescription;
    }
}