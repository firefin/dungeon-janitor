using System.Collections.Generic;
using UnityEngine;

public class PassiveBonusManager : MonoBehaviour
{
    public static PassiveBonusManager Instance { get; private set; }

    private const string SaveKey_Progression = "ProgressionTree_Unlocked";

    [Tooltip("Same list used by ProgressionTreeUI (must be in the same order).")]
    public List<ProgressionNodeSO> allNodes = new();

    [Header("Runtime Bonuses")]
    public float damageMult = 1f;
    public float healthMult = 1f;
    public float speedMult = 1f;
    public float goldMult = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAndRecalculateFromPrefs();
    }

    public void Recalculate(List<ProgressionNodeSO> unlockedNodes)
    {
        int bold = 0, iron = 0, swift = 0, treasure = 0;

        foreach (var node in unlockedNodes)
        {
            if (node == null) continue;

            switch (node.passiveType)
            {
                case PassiveType.BoldAttack:
                    bold = Mathf.Max(bold, node.passiveTier);
                    break;

                case PassiveType.IronBody:
                    iron = Mathf.Max(iron, node.passiveTier);
                    break;

                case PassiveType.SwiftSteps:
                    swift = Mathf.Max(swift, node.passiveTier);
                    break;

                case PassiveType.TreasureHunter:
                    treasure = Mathf.Max(treasure, node.passiveTier);
                    break;
            }
        }

        damageMult = 1f + 0.05f * bold;
        healthMult = 1f + 0.05f * iron;
        speedMult = 1f + 0.05f * swift;
        goldMult = 1f + 0.05f * treasure;
    }

    public void LoadAndRecalculateFromPrefs()
    {
        if (allNodes == null || allNodes.Count == 0)
            return;

        string data = PlayerPrefs.GetString(SaveKey_Progression, string.Empty);

        var unlockedList = new List<ProgressionNodeSO>();
        if (!string.IsNullOrEmpty(data))
        {
            int len = Mathf.Min(data.Length, allNodes.Count);
            for (int i = 0; i < len; i++)
            {
                if (data[i] == '1' && allNodes[i] != null)
                    unlockedList.Add(allNodes[i]);
            }
        }

        Recalculate(unlockedList);
    }
}

