using UnityEngine;

// Class that holds the information about level generation that will be portrayed by the UI.

public class MissionInfo
{
    public int roomCount;
    public int minSize;
    public int maxSize;
    public string difficulty;
}

// Class for managing mission display and generation.

public class MissionManager : MonoBehaviour
{
    public static MissionManager Instance;

    // List of 3 missions to select from.

    public MissionInfo[] missions = new MissionInfo[3];

    // When this script is awoken, we generate missions.

    void Awake()
    {
        Instance = this;
        GenerateMissions();
    }

    void GenerateMissions()
    {
        missions[0] = GenerateSingleMission("Easy");
        missions[1] = GenerateSingleMission("Medium");
        missions[2] = GenerateSingleMission("Hard");
    }

    // Script for generating different levels to display in the mission select screen.

    MissionInfo GenerateSingleMission(string diff)
    {
        MissionInfo m = new MissionInfo();

        switch (diff)
        {
            case "Easy":
                m.roomCount = Random.Range(2, 3);
                m.minSize = 6;
                m.maxSize = 8;
                break;

            case "Medium":
                m.roomCount = Random.Range(3, 5);
                m.minSize = 8;
                m.maxSize = 12;
                break;

            case "Hard":
                m.roomCount = Random.Range(5, 7);
                m.minSize = 12;
                m.maxSize = 20;
                break;
        }

        m.difficulty = diff;
        return m;
    }
}
