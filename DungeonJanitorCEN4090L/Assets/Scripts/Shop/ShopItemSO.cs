using UnityEngine;

[CreateAssetMenu(menuName = "Shop/Shop Item", fileName = "ShopItem")]
public class ShopItemSO : ScriptableObject
{
    public string displayName;
    public Sprite icon;
    public int price;
    [TextArea] public string description;
}
