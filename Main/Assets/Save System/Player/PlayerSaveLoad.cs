using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class PlayerSaveLoad
{
    public static void SaveData(Vector3 position, string[] items, int[] hotkeys, int health, int scene, int saveNum)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.data" + saveNum.ToString();
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(position, items, hotkeys, health, scene, System.DateTime.Now.ToString());

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer(int num)
    {
        string path = Application.persistentDataPath + "/player.data" + num.ToString();
        
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            return data;
        } else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
