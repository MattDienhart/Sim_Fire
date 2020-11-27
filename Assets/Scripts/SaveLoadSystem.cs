using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveLoadSystem
{
    static string path = Application.persistentDataPath + "/save.data";

    public static void SaveGame (GameManager gameManager)
    {
        Debug.Log("Save file created in:" + path);
        BinaryFormatter formatter = new BinaryFormatter();
        FileStream stream = new FileStream(path, FileMode.Create);

        GameData data = new GameData(gameManager);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static GameData LoadGame ()
    {
        if(File.Exists(path))
        {
            Debug.Log("Save file loaded from:" + path);
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            GameData data = (GameData)formatter.Deserialize(stream);
            stream.Close();
            return data;
        }
        else 
        {
            Debug.LogError("Save file not found in:" + path);
            return null;
        }
    }
}
