using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveController
{
    public const string saveDataPath = "/player.saveData";

    public static void Save(string username)
    {
        if (username == "")
            return;

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + saveDataPath;
        FileStream stream = new FileStream(path, FileMode.Create);

        SaveData newData = new SaveData(username);

        formatter.Serialize(stream, newData);
        stream.Close();

        Debug.Log("Saved succesfully to " + path);
    }

    public static void Load()
    {
        string path = Application.persistentDataPath + saveDataPath;

        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            SaveData data = formatter.Deserialize(stream) as SaveData;

            GameController.playerInfo = new PlayerData(data.username, UIController.GetActiveController().testAsMobile ? PlayerData.Device_Type.MB : PlayerData.Device_Type.Null);
            Debug.Log("Loaded PlayerData with username: " + GameController.playerInfo.name + " and deviceType: " + GameController.playerInfo.deviceType);
        }
        else
        {
            Debug.LogError("No save file has been found in " + path);
        }
    }
}
