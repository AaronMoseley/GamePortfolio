using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public float xPos;
    public float yPos;
    public float zPos;

    public string[] items;
    public int[] hotkeyItems;

    public int health;
    public int currScene;

    public string modifiedDate;

    public PlayerData(Vector3 pos, string[] slots, int[] hotkeys, int currHealth, int newScene, string dateTime)
    {
        xPos = pos.x;
        yPos = pos.y;
        zPos = pos.z;

        items = slots;

        hotkeyItems = hotkeys;

        health = currHealth;
        currScene = newScene;

        modifiedDate = dateTime;
    }
}
