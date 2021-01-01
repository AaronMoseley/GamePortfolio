using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class ButtonSoundCheck : MonoBehaviour
{
    Button button;

    void Start()
    {
        button = gameObject.GetComponent<Button>();
    }

    void Update()
    {
        if(!button.interactable && gameObject.GetComponent<EventTrigger>())
        {
            gameObject.GetComponent<EventTrigger>().enabled = false;
        } else if(gameObject.GetComponent<EventTrigger>())
        {
            gameObject.GetComponent<EventTrigger>().enabled = true;
        }
    }
}
