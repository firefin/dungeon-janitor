using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ProgressionTreeUI : MonoBehaviour
{
    private const string SaveKey_Progression = "ProgressionTree_Unlocked";

    [Header("Data")]
    public List<ProgressionNodeSO> nodes = new();  
    public Experience experience;                 
    public TMP_Text xpText;                        

    [Header("Wiring")]
    public ProgressionNodeCard nodePrefab;         

    // Four columns in the progression tree 
    public Transform column1;
    public Transform column2;
    public Transform column3;
    public Transform column4;

    [Header("Passive Bonuses")]
    [Tooltip("Optional: if assigned, will recalc passive multipliers whenever progression is loaded/unlocked.")]
    public PassiveBonusManager passiveBonusManager;

    // runtime state
    private HashSet<ProgressionNodeSO> unlocked = new();
    private List<ProgressionNodeCard> spawnedCards = new();

    void Start()
    {
        LoadProgress();

        RecalculatePassives();

        RefreshXP();
        BuildTree();
        RefreshUI();
    }

    public List<ProgressionNodeSO> GetUnlockedNodes()
    {
        return new List<ProgressionNodeSO>(unlocked);
    }

    void BuildTree()
    {
        ClearColumns();
        spawnedCards.Clear();

        foreach (var node in nodes)
        {
            if (node == null) continue;

            Transform parent = GetColumn(node.columnIndex);
            if (parent == null) continue;

            var card = Instantiate(nodePrefab, parent);
            card.Bind(node, HandleUnlock);

            spawnedCards.Add(card);
        }
    }

    void ClearColumns()
    {
        ClearColumn(column1);
        ClearColumn(column2);
        ClearColumn(column3);
        ClearColumn(column4);
    }

    void ClearColumn(Transform t)
    {
        if (!t) return;

        foreach (Transform child in t)
        {
            Destroy(child.gameObject);
        }
    }

    Transform GetColumn(int index)
    {
        return index switch
        {
            0 => column1,
            1 => column2,
            2 => column3,
            3 => column4,
            _ => null
        };
    }

    void HandleUnlock(ProgressionNodeSO node)
    {
        if (node == null) return;
        if (unlocked.Contains(node)) return;
        if (!experience) return;

        // Check tier progression first
        if (!CanUnlock(node))
        {
            Debug.Log($"Cannot unlock {node.displayName} yet – unlock earlier tiers in this column first.");
            return;
        }

        // Check XP cost
        if (experience.TrySpend(node.cost))
        {
            unlocked.Add(node);
            SaveProgress();

            RecalculatePassives();

            RefreshXP();
            RefreshUI();
        }
        else
        {
            Debug.Log($"Not enough XP to unlock {node.displayName}");
        }
    }

    // must unlock all earlier tiers in the same column
    bool CanUnlock(ProgressionNodeSO node)
    {
        // Tier 0 is always allowed 
        if (node.tierIndex == 0)
            return true;

        for (int i = 0; i < node.tierIndex; i++)
        {
            var prev = GetNode(node.columnIndex, i);
            if (prev == null || !unlocked.Contains(prev))
                return false;
        }

        return true;
    }

    ProgressionNodeSO GetNode(int columnIndex, int tierIndex)
    {
        foreach (var n in nodes)
        {
            if (n == null) continue;
            if (n.columnIndex == columnIndex && n.tierIndex == tierIndex)
                return n;
        }
        return null;
    }

    void SaveProgress()
    {
        if (nodes == null || nodes.Count == 0)
        {
            PlayerPrefs.DeleteKey(SaveKey_Progression);
            return;
        }

        System.Text.StringBuilder sb = new System.Text.StringBuilder(nodes.Count);
        for (int i = 0; i < nodes.Count; i++)
        {
            var node = nodes[i];
            bool isUnlocked = node != null && unlocked.Contains(node);
            sb.Append(isUnlocked ? '1' : '0');
        }

        PlayerPrefs.SetString(SaveKey_Progression, sb.ToString());
        PlayerPrefs.Save();
    }

    void LoadProgress()
    {
        unlocked.Clear();

        string data = PlayerPrefs.GetString(SaveKey_Progression, string.Empty);
        if (string.IsNullOrEmpty(data))
            return;

        int len = Mathf.Min(data.Length, nodes.Count);
        for (int i = 0; i < len; i++)
        {
            if (data[i] == '1' && nodes[i] != null)
            {
                unlocked.Add(nodes[i]);
            }
        }
    }

    void RecalculatePassives()
    {
        if (passiveBonusManager == null) return;

        var unlockedList = new List<ProgressionNodeSO>(unlocked);
        passiveBonusManager.Recalculate(unlockedList);
    }

    void RefreshUI()
    {
        foreach (var card in spawnedCards)
        {
            if (card == null) continue;

            var node = card.Node;
            if (node == null)
            {
                card.SetAsLockedNotAvailable();
                continue;
            }

            bool isUnlocked = unlocked.Contains(node);
            bool canUnlockByTier = CanUnlock(node);
            bool hasEnoughXP = experience == null || experience.XP >= node.cost;

            if (isUnlocked)
            {
                card.SetAsUnlocked();
            }
            else if (canUnlockByTier && hasEnoughXP)
            {
                card.SetAsUnlockable();
            }
            else
            {
                card.SetAsLockedNotAvailable();
            }
        }
    }

    void RefreshXP()
    {
        if (xpText && experience)
            xpText.text = $"XP: {experience.XP}";
    }

    public void ResetProgression()
    {
        unlocked.Clear();

        PlayerPrefs.DeleteKey(SaveKey_Progression);
        PlayerPrefs.Save();

        RecalculatePassives();

        RefreshUI();

        Debug.Log("Progression tree reset.");
    }
}



