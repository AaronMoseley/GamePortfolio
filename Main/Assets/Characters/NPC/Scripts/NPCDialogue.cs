using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    public string[] dialogue;
    public string interactMessage;
    public float timeBetweenCharacters;
    public Image dialogueBackground;
    Text dialogueText;
    int currDialogueIndex = 0;
    int currCharIndex = 0;
    float timer;

    Text interactText;
    CinemachineVirtualCamera vCam;
    InputManager input;
    GameObject player;
    InGameMenuManager menu;
    GameObject gunManage;

    string state = "none";
    bool cantSkip = false;

    void Start()
    {
        interactText = GameObject.FindGameObjectWithTag("Pickup Text").GetComponent<Text>();
        vCam = GameObject.FindGameObjectWithTag("VCam").GetComponent<CinemachineVirtualCamera>();
        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();
        dialogueText = dialogueBackground.gameObject.GetComponentInChildren<Text>();
        player = GameObject.FindGameObjectWithTag("Player");
        menu = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InGameMenuManager>();
        gunManage = player.GetComponentInChildren<GunManager>().gameObject;
    }

    void Update()
    {
        cantSkip = false;

        if(state == "can interact" && input.ButtonDown("Use") && menu.showing == "none")
        {
            ShowDialogue(true);
            interactText.text = "";
            interactText.enabled = false;
            state = "loading text";
            cantSkip = true;
        } else if(state == "loading text")
        {
            if(dialogueText.text == dialogue[currDialogueIndex])
            {
                state = "showing text";
            } else
            {
                timer += Time.deltaTime;

                if(timer >= timeBetweenCharacters)
                {
                    dialogueText.text += dialogue[currDialogueIndex][currCharIndex];
                    currCharIndex++;
                    timer = 0;
                }
            }
        }

        if (Input.anyKeyDown && state == "loading text" && !cantSkip)
        {
            dialogueText.text = dialogue[currDialogueIndex];
        }

        if (state == "showing text" && Input.anyKeyDown)
        {
            currDialogueIndex++;

            if(currDialogueIndex >= dialogue.Length)
            {
                ShowDialogue(false);
            } else
            {
                state = "loading text";
                dialogueText.text = "";
                currCharIndex = 0;
            }
        }
    }

    void ShowDialogue(bool showing)
    {
        dialogueBackground.enabled = showing;
        dialogueText.enabled = showing;
        dialogueText.text = "";
        player.GetComponent<Movement>().enabled = !showing;
        player.GetComponent<Movement>().talking = showing;
        gunManage.SetActive(!showing);
        vCam.Follow = gameObject.transform;

        if(showing)
        {
            state = "loading text";
            interactText.text = "";
            interactText.enabled = false;
            vCam.Follow = gameObject.transform;
        } else
        {
            state = "none";
            vCam.Follow = player.transform;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" && currDialogueIndex < dialogue.Length)
        {
            interactText.text = interactMessage;
            interactText.enabled = true;
            state = "can interact";
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            interactText.text = "";
            interactText.enabled = false;
            state = "none";
        }
    }
}
