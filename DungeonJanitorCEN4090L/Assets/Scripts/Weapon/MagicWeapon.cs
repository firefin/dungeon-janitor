using UnityEngine;

[CreateAssetMenu(fileName = "Magic Weapon", menuName = "Scriptable Objects/Weapon/Magic Weapon")]
public class MagicWeapon : Weapon
{
    [Header("Magic Specific Settings")]
    public int ManaCost;
    public double Cooldown;

    private void OnValidate()
    {
        Class = WeaponClass.Magic;
    }
}
