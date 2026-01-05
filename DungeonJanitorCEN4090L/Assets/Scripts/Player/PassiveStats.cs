using UnityEngine;

public static class PassiveStats
{
    // Bold Attack (increasing player damage output)
    public static int ModifyDamage(int baseDamage)
    {
        if (PassiveBonusManager.Instance == null) return baseDamage;
        return Mathf.RoundToInt(baseDamage * PassiveBonusManager.Instance.damageMult);
    }

    // Iron Body (increasing player health)
    public static int ModifyMaxHealth(int baseMaxHealth)
    {
        if (PassiveBonusManager.Instance == null) return baseMaxHealth;
        return Mathf.RoundToInt(baseMaxHealth * PassiveBonusManager.Instance.healthMult);
    }

    // Swift Steps (increasing player movement speed)
    public static float ModifyMoveSpeed(float baseSpeed)
    {
        if (PassiveBonusManager.Instance == null) return baseSpeed;
        return baseSpeed * PassiveBonusManager.Instance.speedMult;
    }

    // Treasure Hunter (increased gold gain)
    public static int ModifyGoldReward(int baseGold)
    {
        if (PassiveBonusManager.Instance == null) return baseGold;
        return Mathf.RoundToInt(baseGold * PassiveBonusManager.Instance.goldMult);
    }
}

