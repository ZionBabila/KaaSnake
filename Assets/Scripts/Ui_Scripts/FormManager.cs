using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class FormManager : MonoBehaviour
{
    public TMP_InputField firstInput;
    public EventSystem eventSystem;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnEnable()
    {
        firstInput.Select();
    }
    private void Start()
    {
        eventSystem = EventSystem.current;

    }
    private void Update()
    {
        GameObject currentlySelected = eventSystem.currentSelectedGameObject;
        Selectable selectable = currentlySelected.GetComponent<Selectable>();
        if(selectable != null)
        {
            if(Keyboard.current.tabKey.wasPressedThisFrame && Keyboard.current.leftShiftKey.isPressed)
            {
                Selectable previous = selectable.FindSelectableOnUp();
                if(previous != null)
                {
                    previous.Select();
                }
            }
            else if(Keyboard.current.tabKey.wasPressedThisFrame)
            {
                Selectable next = selectable.FindSelectableOnDown();
                if(next != null)
                {
                    next.Select();
                }
        }
    }
    }
    }

