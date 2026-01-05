using UnityEngine;

public class Currency : MonoBehaviour
{
    public int startingGold = 500;
    public int Gold { get; private set; }

    private const string GoldKey = "PlayerGold";

    void Awake()
    {
        // If we've saved gold before, load it.
        // Otherwise, start with startingGold and save that initial value.
        if (PlayerPrefs.HasKey(GoldKey))
        {
            LoadGold();
        }
        else
        {
            Gold = startingGold;
            SaveGold();
        }
    }

    public bool TrySpend(int amt)
    {
        if (Gold < amt)
        {
            return false;
        }

        Gold -= amt;
        SaveGold();   // persist after spending
        return true;
    }

    public void Add(int amt)
    {
        Gold += amt;
        SaveGold();   // persist after earning
    }

    public void SaveGold()
    {
        PlayerPrefs.SetInt(GoldKey, Gold);
        PlayerPrefs.Save();
    }

    public void LoadGold()
    {
        Gold = PlayerPrefs.GetInt(GoldKey, startingGold);
    }
}

