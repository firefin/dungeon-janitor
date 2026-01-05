using UnityEngine;

[CreateAssetMenu(fileName = "Ranged Weapon", menuName = "Scriptable Objects/Weapon/Ranged Weapon")]
public class RangedWeapon : Weapon
{
    [Header("Ranged Specific Properties")]
    
    public int AmmoCapacity;
    public int CurrentAmmoCount;

    public double ReloadTime;



    private void OnValidate()
    {
        Class = WeaponClass.Range;
    }
}
