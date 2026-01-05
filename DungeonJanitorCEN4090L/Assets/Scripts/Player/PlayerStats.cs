using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Player playerData;        // ScriptableObject
    [SerializeField] private healthBar healthBar;      // Scene reference
    [SerializeField] private Transform playerTransform; // actual moving object

    private void Awake()
    {
        // Fallback: if you forget to assign it, use this object
        if (playerTransform == null)
            playerTransform = transform;
    }

    private void Start()
    {
        healthBar.SetMaxHealth(playerData.MaxHealth);

        if (SaveSystem.LoadOnStart && SaveSystem.SaveExists())
        {
            Debug.Log("[PlayerStats] LoadOnStart is true, loading save...");
            LoadPlayer();

            SaveSystem.LoadOnStart = false;
        }
        else
        {
            // Fresh start / New Game path
            playerData.CurrentHealth = playerData.MaxHealth;
            healthBar.SetHealth(playerData.CurrentHealth);
        }
    }


    public void SavePlayer()
    {
        SaveSystem.SavePlayer(playerData, playerTransform);
    }

    public void LoadPlayer()
    {
        PlayerData data = SaveSystem.LoadPlayer();

        if (data == null)
        {
            Debug.LogWarning("LoadPlayer: no save data found.");
            return;
        }

        if (healthBar == null)
        {
            Debug.LogError("LoadPlayer: healthBar reference is null on " + gameObject.name, this);
            return;
        }

        if (data.position == null || data.position.Length < 3)
        {
            Debug.LogError("LoadPlayer: data.position is null or too short");
            return;
        }

        // Restore health
        playerData.CurrentHealth = data.health;
        healthBar.SetHealth(playerData.CurrentHealth);

        // Restore position on the real moving object
        Vector3 pos;
        pos.x = data.position[0];
        pos.y = data.position[1];
        pos.z = data.position[2];

        playerTransform.position = pos;

        Debug.Log($"[PlayerStats] Applied loaded position: {playerTransform.position}");
    }
}
