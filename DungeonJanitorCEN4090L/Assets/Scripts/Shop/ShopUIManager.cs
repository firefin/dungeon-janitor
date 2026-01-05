using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Text;

public enum WeaponType
{
    Sword,
    Bow,
    Staff
}

[System.Serializable]
public class WeaponEntry
{
    public ShopItemSO item;
    public WeaponType type;
    public int tierIndex; // 0..4
}

public class ShopUIManager : MonoBehaviour
{
    private const string SaveKey_ShopWeapons = "Shop_WeaponsPurchased";

    // Separate keys for type-specific multipliers
    private const string Key_MeleeMult = "WeaponMult_Melee";
    private const string Key_RangedMult = "WeaponMult_Ranged";
    private const string Key_MagicMult = "WeaponMult_Magic";

    [Header("Data")]
    public List<ShopItemSO> items = new();

    [Header("Wiring")]
    public Transform gridParent;             
    public ShopItemCard cardPrefab;          
    public Currency currency;                
    public TMP_Text goldText;                

    [Header("Weapon Shop")]
    public Transform swordColumn;            // parent for swords
    public Transform bowColumn;              // parent for bows
    public Transform staffColumn;            // parent for staffs

    [Header("Player (Damage Bonus Target)")]
    public Player playerData;                
    public List<WeaponEntry> weapons = new();    // 15 entries

    private HashSet<ShopItemSO> purchasedWeapons = new();
    private Dictionary<WeaponType, ShopItemSO> equippedByType = new();
    private List<ShopItemCard> weaponCards = new();

    void Awake()
    {
        if (gridParent != null)
        {
            if (swordColumn == null)
            {
                var t = gridParent.Find("SwordColumn");
                if (t != null) swordColumn = t;
            }

            if (bowColumn == null)
            {
                var t = gridParent.Find("BowColumn");
                if (t != null) bowColumn = t;
            }

            if (staffColumn == null)
            {
                var t = gridParent.Find("StaffColumn");
                if (t != null) staffColumn = t;
            }
        }

        LoadShopProgress();
    }

    void Start()
    {
        RefreshGold();
        BuildWeaponShop();
    }

    void Build()
    {
        if (gridParent == null) return;

        foreach (Transform c in gridParent)
            Destroy(c.gameObject);

        foreach (var item in items)
        {
            var card = Instantiate(cardPrefab, gridParent);
            card.Bind(item, HandleBuy);
        }
    }

    void HandleBuy(ShopItemSO item)
    {
        if (!currency) return;

        if (currency.TrySpend(item.price))
        {
            Debug.Log($"Purchased {item.displayName} for ${item.price}");
            RefreshGold();
        }
        else
        {
            Debug.Log($"Not enough gold for {item.displayName}");
        }
    }

    void RefreshGold()
    {
        if (goldText && currency)
            goldText.text = $"Gold: {currency.Gold}";
    }

    void BuildWeaponShop()
    {
        ClearColumn(swordColumn);
        ClearColumn(bowColumn);
        ClearColumn(staffColumn);

        weaponCards.Clear();

        foreach (var weapon in weapons)
        {
            if (weapon == null || weapon.item == null)
                continue;

            Transform parent = GetColumnForType(weapon.type);
            if (!parent) continue;

            var card = Instantiate(cardPrefab, parent);
            card.Bind(weapon.item, HandleWeaponBuy);

            weaponCards.Add(card);
        }

        RefreshAllWeaponCards();
    }

    void ClearColumn(Transform column)
    {
        if (!column) return;
        foreach (Transform c in column)
            Destroy(c.gameObject);
    }

    Transform GetColumnForType(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Sword: return swordColumn;
            case WeaponType.Bow: return bowColumn;
            case WeaponType.Staff: return staffColumn;
            default: return null;
        }
    }

    void HandleWeaponBuy(ShopItemSO item)
    {
        if (item == null) return;

        WeaponEntry entry = GetWeaponEntryForItem(item);
        if (entry == null)
        {
            Debug.LogWarning($"No WeaponEntry found for item {item.name}");
            return;
        }

        if (!CanBuy(entry))
        {
            Debug.Log($"Cannot buy {item.displayName} yet – purchase earlier {entry.type} tiers first.");
            return;
        }

        if (!currency) return;

        if (currency.TrySpend(item.price))
        {
            Debug.Log($"Purchased {item.displayName} ({entry.type} tier {entry.tierIndex}) for ${item.price}");
            RefreshGold();

            purchasedWeapons.Add(item);
            equippedByType[entry.type] = item;

            ApplyWeaponDamageBonus(entry);

            SaveShopProgress();
            RefreshAllWeaponCards();
        }
        else
        {
            Debug.Log($"Not enough gold for {item.displayName}");
        }
    }

    WeaponEntry GetWeaponEntryForItem(ShopItemSO item)
    {
        foreach (var w in weapons)
        {
            if (w != null && w.item == item)
                return w;
        }
        return null;
    }

    bool CanBuy(WeaponEntry entry)
    {
        if (entry.tierIndex == 0)
            return true;

        for (int i = 0; i < entry.tierIndex; i++)
        {
            var prev = GetWeaponByTypeAndTier(entry.type, i);
            if (prev == null || !purchasedWeapons.Contains(prev.item))
                return false;
        }

        return true;
    }

    void ApplyWeaponDamageBonus(WeaponEntry entry)
    {
        if (playerData == null) return;

        int tier = entry.tierIndex + 1;

        float mult = 1f + 0.2f * tier;

        switch (entry.type)
        {
            case WeaponType.Sword:
                playerData.SetMeleeDamageMult(mult);
                PlayerPrefs.SetFloat(Key_MeleeMult, mult);
                break;

            case WeaponType.Bow:
                playerData.SetRangedDamageMult(mult);
                PlayerPrefs.SetFloat(Key_RangedMult, mult);
                break;

            case WeaponType.Staff:
                playerData.SetMagicDamageMult(mult);
                PlayerPrefs.SetFloat(Key_MagicMult, mult);
                break;
        }

        PlayerPrefs.Save();
    }

    WeaponEntry GetWeaponByTypeAndTier(WeaponType type, int tierIndex)
    {
        foreach (var w in weapons)
        {
            if (w != null && w.type == type && w.tierIndex == tierIndex)
                return w;
        }
        return null;
    }

    void RefreshAllWeaponCards()
    {
        foreach (var card in weaponCards)
        {
            if (card == null) continue;

            var item = card.Item;
            if (item == null)
            {
                card.SetAsLockedNotAvailable();
                continue;
            }

            var entry = GetWeaponEntryForItem(item);
            if (entry == null)
            {
                card.SetAsLockedNotAvailable();
                continue;
            }

            bool purchased = purchasedWeapons.Contains(item);
            bool canBuy = CanBuy(entry);

            equippedByType.TryGetValue(entry.type, out ShopItemSO equipped);

            if (purchased)
            {
                if (equipped == item)
                    card.SetAsEquipped();
                else
                    card.SetAsLockedPurchased();
            }
            else
            {
                if (canBuy)
                    card.SetAsBuyable();
                else
                    card.SetAsLockedNotAvailable();
            }
        }
    }

    void SaveShopProgress()
    {
        if (weapons == null || weapons.Count == 0)
        {
            PlayerPrefs.DeleteKey(SaveKey_ShopWeapons);
            PlayerPrefs.Save();
            return;
        }

        StringBuilder sb = new StringBuilder(weapons.Count);
        for (int i = 0; i < weapons.Count; i++)
        {
            var w = weapons[i];
            bool isPurchased = w != null && w.item != null && purchasedWeapons.Contains(w.item);
            sb.Append(isPurchased ? '1' : '0');
        }

        PlayerPrefs.SetString(SaveKey_ShopWeapons, sb.ToString());
        PlayerPrefs.Save();
    }

    void LoadShopProgress()
    {
        purchasedWeapons.Clear();
        equippedByType.Clear();

        string data = PlayerPrefs.GetString(SaveKey_ShopWeapons, string.Empty);
        if (!string.IsNullOrEmpty(data))
        {
            int len = Mathf.Min(data.Length, weapons.Count);
            for (int i = 0; i < len; i++)
            {
                if (data[i] == '1')
                {
                    var w = weapons[i];
                    if (w != null && w.item != null)
                        purchasedWeapons.Add(w.item);
                }
            }
        }

        // Determine equipped as best purchased tier per type
        foreach (WeaponType type in System.Enum.GetValues(typeof(WeaponType)))
        {
            ShopItemSO best = null;
            int bestTier = -1;

            foreach (var w in weapons)
            {
                if (w == null || w.item == null) continue;
                if (w.type != type) continue;
                if (!purchasedWeapons.Contains(w.item)) continue;

                if (w.tierIndex > bestTier)
                {
                    bestTier = w.tierIndex;
                    best = w.item;
                }
            }

            if (best != null)
                equippedByType[type] = best;

            // apply multiplier to Player based on best tier 
            if (bestTier >= 0 && playerData != null)
            {
                float mult = 1f + 0.2f * (bestTier + 1);

                switch (type)
                {
                    case WeaponType.Sword:
                        playerData.SetMeleeDamageMult(mult);
                        PlayerPrefs.SetFloat(Key_MeleeMult, mult);
                        break;
                    case WeaponType.Bow:
                        playerData.SetRangedDamageMult(mult);
                        PlayerPrefs.SetFloat(Key_RangedMult, mult);
                        break;
                    case WeaponType.Staff:
                        playerData.SetMagicDamageMult(mult);
                        PlayerPrefs.SetFloat(Key_MagicMult, mult);
                        break;
                }
            }
        }

        PlayerPrefs.Save();
    }

    public void ResetWeaponPurchases()
    {
        purchasedWeapons.Clear();
        equippedByType.Clear();

        PlayerPrefs.DeleteKey(SaveKey_ShopWeapons);

        // Reset multipliers
        PlayerPrefs.SetFloat(Key_MeleeMult, 1f);
        PlayerPrefs.SetFloat(Key_RangedMult, 1f);
        PlayerPrefs.SetFloat(Key_MagicMult, 1f);
        PlayerPrefs.Save();

        if (playerData != null)
        {
            playerData.SetMeleeDamageMult(1f);
            playerData.SetRangedDamageMult(1f);
            playerData.SetMagicDamageMult(1f);
        }

        RefreshAllWeaponCards();
        Debug.Log("Shop weapon purchases reset.");
    }
}


