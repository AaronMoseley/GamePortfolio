using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsData : MonoBehaviour
{
    public Resolution resolution;
    public float volume;
    public bool fullscreen;
    public int quality;
    public string[][] keyBindings;

    InputManager input;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (GameObject.FindGameObjectsWithTag("Stored Settings").Length > 1)
        {
            Destroy(gameObject);
        }

        input = GameObject.FindGameObjectWithTag("Game Manager").GetComponent<InputManager>();

        keyBindings = new string[input.buttons.Count + input.hotkeys.Count][];

        for (int i = 0; i < keyBindings.Length; i++)
        {
            keyBindings[i] = new string[2];
        }
    }

    public void UpdateBindings()
    {
        for (int i = 0; i < input.buttons.Count; i++)
        {
            keyBindings[i][0] = input.buttons[i].name;
            keyBindings[i][1] = input.buttons[i].code.ToString();
        }
    }
}
