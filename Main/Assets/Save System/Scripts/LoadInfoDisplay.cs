using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class LoadInfoDisplay : MonoBehaviour
{
    public Text displayText;
    public int saveNum;
    public bool loadButton;

    AreaList areas;

    void Start()
    {
        areas = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<AreaList>();

        UpdateText();
    }

    void LateUpdate()
    {
        if (gameObject.GetComponent<Button>().interactable && loadButton && PlayerSaveLoad.LoadPlayer(saveNum) == null)
        {
            gameObject.GetComponent<Button>().interactable = false;
        }

        if(!gameObject.GetComponent<Button>().interactable && gameObject.GetComponent<EventTrigger>())
        {
            gameObject.GetComponent<EventTrigger>().enabled = false;
        } else if(gameObject.GetComponent<Button>().interactable && gameObject.GetComponent<EventTrigger>())
        {
            gameObject.GetComponent<EventTrigger>().enabled = true;
        }
    }

    public void UpdateText()
    {
        if (PlayerSaveLoad.LoadPlayer(saveNum) != null)
        {
            PlayerData data = PlayerSaveLoad.LoadPlayer(saveNum);
            int health = data.health;
            int areaNum = data.currScene;
            string lastChanged = data.modifiedDate;

            displayText.text = "Health: " + health + "\nArea: " + areas.areas[areaNum] + "\nLast Modified: " + lastChanged;
        }
    }
}
