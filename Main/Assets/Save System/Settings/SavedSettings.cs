using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedSettings
{
    public int resHeight;
    public int resWidth;
    public bool fullscreen;
    public float volume;
    public int quality;
    public string[][] bindings;

    public SavedSettings(SettingsData data)
    {
        resHeight = data.resolution.height;
        resWidth = data.resolution.width;
        fullscreen = data.fullscreen;
        volume = data.volume;
        quality = data.quality;
        bindings = data.keyBindings;
    }
}
