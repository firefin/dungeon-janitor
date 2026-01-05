using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    
    public static bool LoadOnStart = false;

    public static void SavePlayer(Player player, Transform playerTransform)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.fun";
        FileStream stream = new FileStream(path, FileMode.Create);

        PlayerData data = new PlayerData(player, playerTransform);

        Debug.Log($"[SaveSystem] Saving player at position {playerTransform.position} to {path}");

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static PlayerData LoadPlayer()
    {
        string path = Application.persistentDataPath + "/player.fun";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            PlayerData data = formatter.Deserialize(stream) as PlayerData;
            stream.Close();

            Debug.Log($"[SaveSystem] Loaded player data: health={data.health}, " +
                      $"pos=({data.position[0]}, {data.position[1]}, {data.position[2]})");

            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }

    public static bool SaveExists()
    {
        string path = Application.persistentDataPath + "/player.fun";
        return File.Exists(path);
    }
}
