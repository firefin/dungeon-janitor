using UnityEngine;

[System.Serializable]
public class PlayerData
{
    public int health;
    public float[] position;

    public PlayerData(Player player, Transform playerTransform)
    {
        health = player.CurrentHealth;

        position = new float[3];
        Vector3 pos = playerTransform.position;
        position[0] = pos.x;
        position[1] = pos.y;
        position[2] = pos.z;
    }
}
