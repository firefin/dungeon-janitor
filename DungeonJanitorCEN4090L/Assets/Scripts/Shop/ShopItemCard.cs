using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemCard : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text priceText;
    public Button buyButton;

    public TMP_Text buyButtonLabel;

    public GameObject lockIcon;

    private ShopItemSO data;
    private System.Action<ShopItemSO> onBuy;

    public ShopItemSO Item => data;

    public void Bind(ShopItemSO item, System.Action<ShopItemSO> onBuyClicked)
    {
        data = item;
        onBuy = onBuyClicked;

        if (icon)
        {
            icon.sprite = item.icon;
        }

        if (nameText)
        {
            nameText.text = item.displayName;
        }

        if (priceText)
        {
            priceText.text = $"${item.price}";
        }

        if (buyButton)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(() => onBuy?.Invoke(data));
        }

        SetAsBuyable();
    }

    private void OnEnable()
    {
        if (data != null)
        {
            SetAsBuyable();
        }
    }

    public void SetAsBuyable()
    {
        if (buyButton) buyButton.interactable = true;
        if (buyButtonLabel) buyButtonLabel.text = "Buy";
        if (lockIcon) lockIcon.SetActive(false);
    }

    public void SetAsEquipped()
    {
        if (buyButton) buyButton.interactable = false;
        if (buyButtonLabel) buyButtonLabel.text = "Equipped";
        if (lockIcon) lockIcon.SetActive(false);
    }

    public void SetAsLockedPurchased()
    {
        if (buyButton) buyButton.interactable = false;
        if (buyButtonLabel) buyButtonLabel.text = "";
        if (lockIcon) lockIcon.SetActive(true);
    }

    public void SetAsLockedNotAvailable()
    {
        if (buyButton) buyButton.interactable = false;
        if (buyButtonLabel) buyButtonLabel.text = "Locked";
        if (lockIcon) lockIcon.SetActive(true);
    }
}

