using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProgressionNodeCard : MonoBehaviour
{
    [Header("UI")]
    public Image icon;
    public TMP_Text nameText;
    public TMP_Text costText;
    public Button unlockButton;

    public TMP_Text buttonLabel;

    public GameObject lockIcon;

    private ProgressionNodeSO data;
    private System.Action<ProgressionNodeSO> onUnlock;

    public ProgressionNodeSO Node => data;

    public void Bind(ProgressionNodeSO node, System.Action<ProgressionNodeSO> onUnlockClicked)
    {
        data = node;
        onUnlock = onUnlockClicked;

        if (icon && node.icon != null)
        {
            icon.sprite = node.icon;
        }

        if (nameText)
        {
            nameText.text = node.displayName;
        }

        if (costText)
        {
            costText.text = $"Cost: {node.cost}";
        }

        if (unlockButton)
        {
            unlockButton.onClick.RemoveAllListeners();
            unlockButton.onClick.AddListener(() => onUnlock?.Invoke(data));
        }

        SetAsLockedNotAvailable();
    }

    public void Bind(ProgressionNodeSO node, bool isUnlocked, System.Action<ProgressionNodeSO> onUnlockClicked)
    {
        Bind(node, onUnlockClicked);

        if (isUnlocked)
        {
            SetAsUnlocked();
        }
        else
        {
            SetAsUnlockable(); 
        }
    }

    // Node cannot be unlocked yet 
    public void SetAsLockedNotAvailable()
    {
        if (unlockButton) unlockButton.interactable = false;
        if (buttonLabel) buttonLabel.text = "Locked";
        if (lockIcon) lockIcon.SetActive(true);
        if (costText) costText.text = $"Cost: {data?.cost ?? 0}";
    }

    // Node is available to unlock 
    public void SetAsUnlockable()
    {
        if (unlockButton) unlockButton.interactable = true;
        if (buttonLabel) buttonLabel.text = "Unlock";
        if (lockIcon) lockIcon.SetActive(false);
        if (costText) costText.text = $"Cost: {data?.cost ?? 0}";
    }

    // Node has been unlocked
    public void SetAsUnlocked()
    {
        if (unlockButton) unlockButton.interactable = false;
        if (buttonLabel) buttonLabel.text = "Unlocked";
        if (lockIcon) lockIcon.SetActive(false);
        if (costText) costText.text = "Unlocked";
    }
}


