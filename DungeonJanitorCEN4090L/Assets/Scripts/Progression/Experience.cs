using UnityEngine;

public class Experience : MonoBehaviour
{
    public int startingXP = 500;
    public int XP { get; private set; }

    private const string XPKey = "PlayerXP";

    void Awake()
    {
        // If we've saved XP before, load it.
        // Otherwise, start from startingXP and save that as the initial value.
        if (PlayerPrefs.HasKey(XPKey))
        {
            LoadXP();
        }
        else
        {
            XP = startingXP;
            SaveXP();
        }
    }

    public bool TrySpend(int amount)
    {
        if (XP < amount) return false;

        XP -= amount;
        SaveXP();           //persist after spending
        return true;
    }

    public void Add(int amount)
    {
        XP += amount;
        SaveXP();           //persist after gaining XP
    }

    public void SaveXP()
    {
        PlayerPrefs.SetInt(XPKey, XP);
        PlayerPrefs.Save();
    }

    public void LoadXP()
    {
        // Use startingXP as default if the key somehow doesn't exist yet
        XP = PlayerPrefs.GetInt(XPKey, startingXP);
    }
}

