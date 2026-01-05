using UnityEngine;

public enum PassiveType
{
    BoldAttack,
    IronBody,
    SwiftSteps,
    TreasureHunter
}

[CreateAssetMenu(menuName = "Progression/Node", fileName = "ProgressionNode")]
public class ProgressionNodeSO : ScriptableObject
{
    [Header("Display")]
    public string displayName;
    public Sprite icon;
    public int cost;
    [TextArea] public string placeholderEffect;

    [Header("Passive Unlock")]
    public PassiveType passiveType;

    // I-V maps to 1-5. Tier 0 means "not a passive tier node"
    [Range(0, 5)]
    public int passiveTier = 0;

    [Header("Tree Position")]
    // Column this node belongs to (0–4 for your 5 columns)
    public int columnIndex;

    // Tier within that column (0..4)
    public int tierIndex;
}