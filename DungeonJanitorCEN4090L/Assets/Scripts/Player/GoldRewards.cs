using UnityEngine;

public static class GoldRewards
{
    // Call this whenever an enemy dies and you want to award gold.
    // Example later: GoldRewards.AwardGold(currency, enemyGoldValue);
    public static void AwardGold(Currency currency, int baseGold)
    {
        if (currency == null)
        {
            Debug.LogWarning("GoldRewards.AwardGold called with null Currency.");
            return;
        }

        int finalGold = PassiveStats.ModifyGoldReward(baseGold);
        currency.Add(finalGold);
    }
}
