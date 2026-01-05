using UnityEditor.Playables;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "Scriptable Objects/Player")]
public class Player : ScriptableObject
{

    [Header("Equipment")]
    [SerializeField, Tooltip("Should always be equipped")] private Weapon broomWeapon;

    [Header("Combat Stats")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxMana = 100;

    [Header("Weapon Damage Bonuses (Shop)")]
    [SerializeField] private float meleeDamageMult = 1f;
    [SerializeField] private float rangedDamageMult = 1f;
    [SerializeField] private float magicDamageMult = 1f;

    public float MeleeDamageMult => meleeDamageMult;
    public float RangedDamageMult => rangedDamageMult;
    public float MagicDamageMult => magicDamageMult;

    public void SetMeleeDamageMult(float mult) => meleeDamageMult = Mathf.Max(1f, mult);
    public void SetRangedDamageMult(float mult) => rangedDamageMult = Mathf.Max(1f, mult);
    public void SetMagicDamageMult(float mult) => magicDamageMult = Mathf.Max(1f, mult);

    private const string Key_MeleeMult = "WeaponMult_Melee";
    private const string Key_RangedMult = "WeaponMult_Ranged";
    private const string Key_MagicMult = "WeaponMult_Magic";

    [SerializeField] private int baseMaxHealth = 100;
    public int BaseMaxHealth => baseMaxHealth;

    public int CurrentHealth;
    public int CurrentMana;

    [Header("Character Stats")]
    public int Level = 1;
    public int Experience;

    public double Damage;
    public double BaseStrength;
    public float Speed;

    [Header("Equipment Slots")]
    public Weapon Weapon;
    public Armor Armor;

    [Header("Abilities")]
    [SerializeField] private int maxAbilitySlots = 3;
    [SerializeField] private AbilityDefinition[] equippedAbilities;
    public AbilityDefinition[] EquippedAbilities => equippedAbilities;

    [Header("Weapon Damage Bonus (Shop)")]
    [SerializeField] private float weaponDamageMult = 1f;
    public float WeaponDamageMult => weaponDamageMult;

    // Properties
    public int MaxHealth => maxHealth;
    public int MaxMana => maxMana;
    public Weapon BroomWeapon => broomWeapon = broomWeapon != null ? broomWeapon : Resources.Load<Weapon>("Weapons/Melee/Broom");

    public float HealthPercentage => maxHealth > 0 ? ((float)CurrentHealth / MaxHealth) : 0f;
    public bool IsDead => CurrentHealth <= 0;

    public void Initialize()
    {
        maxHealth = baseMaxHealth;
        CurrentHealth = maxHealth;
        CurrentMana = maxMana;
        Level = 1;
        Experience = 0;

        meleeDamageMult = PlayerPrefs.GetFloat(Key_MeleeMult, 1f);
        rangedDamageMult = PlayerPrefs.GetFloat(Key_RangedMult, 1f);
        magicDamageMult = PlayerPrefs.GetFloat(Key_MagicMult, 1f);

        if (equippedAbilities == null || equippedAbilities.Length != maxAbilitySlots)
        {
            equippedAbilities = new AbilityDefinition[maxAbilitySlots];
        }

        ApplyPassiveStats();
    }

    public AbilityDefinition GetAbility(int index)
    {
        if (index < 0 || index >= equippedAbilities.Length)
        {
            Debug.LogError($"GetAbility: index out of range. Index Value: {index} | Max Array Index: {equippedAbilities.Length-1}");
            return null;
        }
        return equippedAbilities[index];
    }

    public void ApplyPassiveStats()
    {
        float pct = MaxHealth > 0 ? (float)CurrentHealth / MaxHealth : 1f;

        maxHealth = PassiveStats.ModifyMaxHealth(baseMaxHealth);
        CurrentHealth = Mathf.Clamp(Mathf.RoundToInt(MaxHealth * pct), 0, MaxHealth);
    }

    public void SetWeaponDamageMult(float mult)
    {
        weaponDamageMult = Mathf.Max(1f, mult);
    }

    public void EquipAbility(AbilityDefinition ability, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= equippedAbilities.Length)
        {
            Debug.LogError($"EquipAbility: Invalid ability slot index: {slotIndex}");
            return;
        }
        equippedAbilities[slotIndex] = ability;
    }
}
