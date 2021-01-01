using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class InputChanger : MonoBehaviour
{
    int buttonNum;
    InputManager inputManager;
    public string buttonName;

    public Text buttonText;
    public Text buttonTitle;

    public bool selected;

    GameObject[] otherChangers;
    KeyCode[] mouseKeys;

    void Start()
    {
        inputManager = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        otherChangers = GameObject.FindGameObjectsWithTag("Input Changer");
        mouseKeys = new KeyCode[] { KeyCode.Mouse0, KeyCode.Mouse1, KeyCode.Mouse2, KeyCode.Mouse3, KeyCode.Mouse4, KeyCode.Mouse5, KeyCode.Mouse6 };

        for (int i = 0; i < inputManager.buttons.Count; i++)
        {
            if(inputManager.buttons[i].name == buttonName)
            {
                buttonNum = i;
            }
        }

        buttonText.text = inputManager.buttons[buttonNum].code.ToString();
        buttonTitle.text = inputManager.buttons[buttonNum].name;
    }

    void OnGUI()
    {
        Event e = Event.current;

        if ((e.isKey || e.isMouse) && selected)
        {
            if (e.isMouse)
            {
                if (e.type == EventType.MouseDown)
                {
                    for (int i = 0; i < mouseKeys.Length; ++i)
                    {
                        if (Input.GetKeyDown(mouseKeys[i]))
                        {
                            ChangeCode(mouseKeys[i]);
                            break;
                        }
                    }
                }
            }
            else
            {
                ChangeCode(e.keyCode);
            }

            buttonText.text = inputManager.buttons[buttonNum].code.ToString();
        }
    }

    public void Select()
    {
        selected = !selected;

        for(int i = 0; i < otherChangers.Length; i++)
        {
            if(otherChangers[i] != gameObject && otherChangers[i].GetComponent<InputChanger>().selected)
            {
                otherChangers[i].GetComponent<InputChanger>().selected = false;
            }
        }
    }

    public void ChangeCode(KeyCode code)
    {
        inputManager.buttons[buttonNum].code = code;
        selected = false;
    }
}
