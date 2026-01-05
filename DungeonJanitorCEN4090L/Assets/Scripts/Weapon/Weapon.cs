using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Weapon")]
public class Weapon : ScriptableObject
{
    // TODO: Determine access modifier state for each variable

    public string WeaponName;
    public Sprite Icon;
    public int Damage;
    // public int AmmoCount; // For ranged weapons
    public int WeaponLevel;

    public double AttackSpeed;
    public double Range;

    public WeaponClass Class;

    //knockback effects later


    public enum WeaponClass
    {
        Melee,
        Magic,
        Range
    }

    public Weapon()
    {

    }

    ~Weapon()
    {

    }

    private void OnValidate()
    {
#if UNITY_EDITOR // Preprocess only in the editor

        string path = AssetDatabase.GetAssetPath(this);
        if (!string.IsNullOrEmpty(path))
        {
            string fileName = System.IO.Path.GetFileNameWithoutExtension(path);
            if (WeaponName != fileName)
            {
                WeaponName = fileName;
                EditorUtility.SetDirty(this); // Mark the object as dirty to ensure the change is saved
            }
        }
#endif
    }
}
