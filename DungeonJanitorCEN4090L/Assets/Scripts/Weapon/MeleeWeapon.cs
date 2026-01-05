using UnityEngine;

[CreateAssetMenu(fileName = "Melee Weapon", menuName = "Scriptable Objects/Weapon/Melee Weapon")]
public class MeleeWeapon : Weapon
{
    private void OnValidate()
    {
        Class = WeaponClass.Melee;
    }
}
