using UnityEngine;
using TMPro;
// Header file for CleanableObject class which manages the janitor side of Dungeon Janitor. Uses TMPro to handle the text prompt.
public class CleanableObject : MonoBehaviour
{
    public int featureID;
    public Vector2Int tilePosition;

    // The Object will store its local coordinates as well as a direct reference to its parent room to make search logic easier down the line.
    public Vector2Int localTilePosition;
    public Room parentRoom;

    public int experienceReward = 5;
    [Header("World Prompt")]
    public TextMeshPro worldPromptPrefab;
    // I have this set to false to disable floating text by default.
    public bool useWorldSpacePrompt = false;
    
    private LevelManager levelManager;
    private SpriteRenderer spriteRenderer;
    private bool isPlayerNearby = false;
    private bool isCleaned = false;
    private GameObject worldPrompt;

void Start()
    {
        levelManager = FindObjectOfType<LevelManager>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    void Update()
    {
        // Check if the player is nearby, the mess is not cleaned, and the player is holding 'E'.
        if (isPlayerNearby && !isCleaned && Input.GetKeyDown(KeyCode.E))
        {
            Clean();
        }
    }
    
    public void Clean()
    {
        if (isCleaned) return;

        isCleaned = true;

        // First, find the tile and clear its feature.

        if (parentRoom != null)
        {
            Tile tile = parentRoom.GetTile(localTilePosition.x, localTilePosition.y);
            if (tile != null)
            {
                tile.FeatureID = FeatureID.NONE;
            }
        }
        
        // Destroy the gameObject on this current tile. For safety, check to ensure spriteRenderer is initialized before attempting animations.
        if (spriteRenderer != null)
        {
            StartCoroutine(FadeOutAndDestroy());
        }
        else
        {
            Destroy(gameObject);
        }
        Debug.Log($"Cleaned {gameObject.name} at position {tilePosition}");

        // Award the player some XP.
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            // TODO: IMPLEMENT AddExperience METHOD TO PlayerStats
            Debug.Log($"Granted Player {experienceReward} XP for cleaning {gameObject.name}");
        }
    }
    
    // Function to provide visual feedback when cleaning a mess.
    private System.Collections.IEnumerator FadeOutAndDestroy()
    {
        // These values are arbitrary.
        float fadeDuration = 0.3f;
        float elapsed = 0f;
        Color originalColor = spriteRenderer.color;
        // Using deltaTime to prevent framerate from making this script wonky.
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            // Lerp is short for linear interpolation, using this to calculate the percentage of fade for the fade script.
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        
        Destroy(gameObject);
    }
    
    // Functions used to prompt the player to clean messes as well as set isPlayerNearby to true or false.
    // TODO: Make this script not active when enemies are in the room.
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            ShowPrompt();
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            HidePrompt();
        }
    }
    // Function to display the text prompt when a player can clean.
    private void ShowPrompt()
{
    if (useWorldSpacePrompt)
    {
        if (worldPrompt == null && worldPromptPrefab != null)
        {
            // Instantiate the 3D text
            worldPrompt = Instantiate(worldPromptPrefab.gameObject, transform);
            worldPrompt.transform.localPosition = new Vector3(0, 1f, 0); // raise above sprite
            worldPrompt.transform.localRotation = Quaternion.identity;

            // TMP 3D text is tiny by default; scale it up
            worldPrompt.transform.localScale = Vector3.one * 0.2f;

            // Set the text
            TextMeshPro tmp = worldPrompt.GetComponent<TextMeshPro>();
            if (tmp != null)
            {
                tmp.text = "Press E to clean!";
                tmp.alignment = TextAlignmentOptions.Center;
            }

            // Make it face the main camera
            if (Camera.main != null)
            {
                worldPrompt.transform.rotation = Quaternion.LookRotation(worldPrompt.transform.position - Camera.main.transform.position);
            }
        }
    }
}
    // Function to hide the prompt when player stops colliding with a mess.
    private void HidePrompt()
    {
        if (useWorldSpacePrompt && worldPrompt != null)
        {
            Destroy(worldPrompt);
        }
    }
}
