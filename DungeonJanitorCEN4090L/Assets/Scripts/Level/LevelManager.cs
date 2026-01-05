using UnityEngine;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

public class LevelManager : MonoBehaviour
{
    private GameObject player;

    [Header("Features")]
    public GameObject wallPrefab;
    public GameObject tablePrefab;
    public GameObject decorationPrefab;
    public GameObject carpetPrefab;
    public GameObject chairPrefab;
    public GameObject chestPrefab;

    [Header("Cleanable Messes")]
    public GameObject dirtPilePrefab;
    public GameObject trashPrefab;
    public GameObject bloodStainPrefab;
    public GameObject brokenGlassPrefab;
    public GameObject skeletonPrefab;

    [Header("Generation Settings")]
    public float tileSize = 1f;
    public float roomSpacing = 20f;

    [Header("Current Mission")]
    public MissionDifficulty currentDifficulty;
    public Level CurrentLevel { get; private set; }

    // Storing these so we can track when players are allowed to leave the room
    private int totalMesses = 0;
    private int messesCleanedCount = 0;

    // Dictionary that maps IDs to prefab objects
    private Dictionary<int, GameObject> featurePrefabs;

    void Awake()
    {
        InitializeFeaturePrefabs();
    }

    void Start()
    {
        int choice = SelectedMission.index;
        MissionInfo m = MissionManager.Instance.missions[choice];

        // Convert MissionInfo into MissionDifficulty logic
        if (m.difficulty == "Easy")
        { 
            currentDifficulty = MissionDifficulty.Easy;
        }
        else if (m.difficulty == "Medium")
        {
            currentDifficulty = MissionDifficulty.Medium;
        }
        else 
        {
            currentDifficulty = MissionDifficulty.Hard;
        }
        player = GameObject.FindWithTag("Player");
        
        GenerateLevel(currentDifficulty);

        // Commenting out SampleLevel call here as well as implementation down later, if we need to test we can uncomment
        // GenerateSampleLevel();

        SpawnFeatures();
        SpawnPlayer();
    }

    void InitializeFeaturePrefabs()
    {
        featurePrefabs = new Dictionary<int, GameObject>
        {
            { FeatureID.TABLE, tablePrefab },
            { FeatureID.WALL, wallPrefab },
            { FeatureID.DECORATION, decorationPrefab },
            { FeatureID.CARPET, carpetPrefab },
            { FeatureID.CHAIR, chairPrefab },
            { FeatureID.CHEST, chestPrefab },
	        { FeatureID.DIRT_PILE, dirtPilePrefab },
            { FeatureID.TRASH, trashPrefab },
            { FeatureID.BLOOD_STAIN, bloodStainPrefab },
            { FeatureID.BROKEN_GLASS, brokenGlassPrefab },
	        { FeatureID.SKELETON, skeletonPrefab }
        };
    }


    // Sample Level Generator with 2 rooms. This hard sets rooms, only used for testing purposes
    // void GenerateSampleLevel()
    // {
    //     CurrentLevel = new Level(2);
    //     Room room1 = new Room(10, 10, new Vector2Int(0, 0), tileSize);
    //     room1.GenerateBasicRoom();
    //     room1.SetFeature(5, 5, FeatureID.TABLE);
    //     room1.SetFeature(3, 3, FeatureID.CHAIR);
    //     room1.SetFeature(7, 7, FeatureID.CHEST);
    //     Room room2 = new Room(8, 8, new Vector2Int(15, 0), tileSize);
    //     room2.GenerateBasicRoom();
    //     room2.SetFeature(4, 4, FeatureID.DECORATION);
    //     CurrentLevel.AddRoom(0, room1);
    //     CurrentLevel.AddRoom(1, room2);
    //     Debug.Log("Level generated with " + CurrentLevel.Rooms.Length + " rooms");
    // }

    void GenerateLevel(MissionDifficulty difficulty)
    {
        int roomCount = 0;
        int minSize = 0;
        int maxSize = 0;

        switch (difficulty)
        {
            case MissionDifficulty.Easy:
                roomCount = 2;
                minSize = 6; maxSize = 8;
                break;

            case MissionDifficulty.Medium:
                roomCount = Random.Range(3, 5);
                minSize = 8; maxSize = 12;
                break;

            case MissionDifficulty.Hard:
                roomCount = Random.Range(5, 7);
                minSize = 12; maxSize = 20;
                break;
        }

        CurrentLevel = new Level(roomCount);

        float xOffset = 0f;

        for (int i = 0; i < roomCount; i++)
        {
            int w = Random.Range(minSize, maxSize + 1);
            int h = Random.Range(minSize, maxSize + 1);

            Room room = new Room(w, h, new Vector2(xOffset, 0), tileSize);

            room.GenerateBasicRoom();        // walls
            GenerateRandomFeatures(room);    // tables, chairs, decorations
            GenerateRandomMesses(room);      // glass, skeletons, etc.

            CurrentLevel.AddRoom(i, room);

            xOffset += (w * tileSize) + roomSpacing;
        }
    }

    void GenerateRandomFeatures(Room room)
    {
        int count = Random.Range(2, 6);

        for (int i = 0; i < count; i++)
        {
            Tile tile = GetRandomEmptyTile(room);
            if (tile == null)
            {
                return;
            }
            // This code is lazy and bad. Walls are considered features in the enum and can be generated on the floor, potentially soft-locking a player.
            // The proper solution is for me to redo every prefab and script that references Features and fix the FeatureIDs, but that is a lot of work that we don't have time for.
            // I am triaging this, acknowledging it is a sloppy solution.
            int feature = Random.Range(FeatureID.TABLE, FeatureID.CHEST + 1);
            if (feature == FeatureID.WALL)
                feature = FeatureID.TABLE;

            tile.FeatureID = feature;
        }   
    }

    void GenerateRandomMesses(Room room)
    {
        int messCount = Random.Range(3, 7);

        for (int i = 0; i < messCount; i++)
        {
            Tile tile = GetRandomEmptyTile(room);
            if (tile == null)
            {
                return;
            }

            tile.FeatureID = Random.Range(FeatureID.DIRT_PILE, FeatureID.SKELETON + 1);
            totalMesses++;
        }
    }

    Tile GetRandomEmptyTile(Room room)
    {
        List<Tile> empty = new List<Tile>();

        for (int x = 1; x < room.Width - 1; x++)
        {
            for (int y = 1; y < room.Height - 1; y++)
            {
                Tile t = room.GetTile(x, y);

                if (!t.IsWall && t.FeatureID == FeatureID.NONE)
                    empty.Add(t);
            }
        }

        if (empty.Count == 0)
            return null;

        return empty[Random.Range(0, empty.Count)];
    }

    // Now that the level is properly spawned in, we add features
    void SpawnFeatures()
    {
        Tile[] tiles = CurrentLevel.GetAllTilesWithFeatures();

        foreach (Tile tile in tiles)
        {
            if (!featurePrefabs.TryGetValue(tile.FeatureID, out GameObject prefab))
            {
                continue;
            }

            // Build a new world position based off of tile positions and their size
            Room parentRoom = tile.parentRoom;

            Vector2 worldPos = (parentRoom.Position + (Vector2)tile.Position) * tileSize;

            GameObject obj = Instantiate(prefab, worldPos, Quaternion.identity);
            obj.transform.parent = transform;
            obj.name = GetFeatureName(tile.FeatureID);

            if (ShouldHaveCollider(tile.FeatureID))
                EnsureCollider(obj);

            // Attach CleanableObject info if it's a Cleanable Object
            CleanableObject clean = obj.GetComponent<CleanableObject>();
            if (clean != null)
            {
                clean.featureID = tile.FeatureID;
                clean.localTilePosition = tile.Position;
                clean.parentRoom = parentRoom;
            }
        }
    }

    // This function should be an exhaustive list of which features block movement
    bool ShouldHaveCollider(int featureID)
    {
        return featureID == FeatureID.TABLE || featureID == FeatureID.WALL || featureID == FeatureID.CHEST || featureID == FeatureID.CHAIR;
    }
    // This function forces colliders to spawn (for Rigidbody purposes)
    void EnsureCollider(GameObject obj)
    {
        if (obj.GetComponent<Collider2D>() == null)
        {
            BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
            SpriteRenderer sprite = obj.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                collider.size = sprite.bounds.size;
            }
        }
    }
    // Player spawn script
    void SpawnPlayer()
    {
        Room room = CurrentLevel.Rooms[0];
        Vector2 spawnPos = room.TileToWorldPosition(room.Width / 2, room.Height / 2);
        player.transform.position = spawnPos;
    }

    // Returns Feature's name
    string GetFeatureName(int featureID)
    {
        switch (featureID)
        {
            case FeatureID.TABLE: return "Table";
            case FeatureID.WALL: return "Wall";
            case FeatureID.DECORATION: return "Decoration";
            case FeatureID.CARPET: return "Carpet";
            case FeatureID.CHAIR: return "Chair";
            case FeatureID.CHEST: return "Chest";
	        case FeatureID.DIRT_PILE: return "Dirt Pile";
            case FeatureID.TRASH: return "Trash";
            case FeatureID.BLOOD_STAIN: return "Blood Stain";
            case FeatureID.BROKEN_GLASS: return "Broken Glass";
	        case FeatureID.SKELETON: return "Skeleton";
            default: return "Feature";
        }
    }
}