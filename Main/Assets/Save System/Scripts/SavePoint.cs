using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SavePoint : MonoBehaviour
{
    public string useMessage;

    Text pickupText;
    bool canSave = false;
    InputManager input;
    PlayerSaveSystem saveSystem;
    InGameMenuManager menu;
    
    void Start()
    {
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        saveSystem = GameObject.FindGameObjectWithTag("Save System").GetComponent<PlayerSaveSystem>();
        pickupText = GameObject.FindGameObjectWithTag("Pickup Text").GetComponent<Text>();
        menu = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>();
    }

    void Update()
    {
        if(menu.showing == "saves" && input.ButtonDown("Escape"))
        {
            menu.ShowSaves(false);
            menu.showing = "none";
        }
        
        if(canSave && input.ButtonDown("Use"))
        {
            menu.ShowSaves(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            pickupText.text = useMessage;
            pickupText.enabled = true;
            canSave = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            pickupText.text = "";
            pickupText.enabled = false;
            canSave = false;
        }
    }
}
