using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSettingsProcess
{
    public static void SaveSettings(SettingsData data)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/settings.data";
        FileStream stream = new FileStream(path, FileMode.Create);

        SavedSettings dataToSave = new SavedSettings(data);

        formatter.Serialize(stream, dataToSave);
        stream.Close();
    }

    public static SavedSettings LoadSettings()
    {
        string path = Application.persistentDataPath + "/settings.data";

        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SavedSettings newData = formatter.Deserialize(stream) as SavedSettings;

            stream.Close();

            return newData;
        } else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
