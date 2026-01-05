using UnityEngine;

public class MissionMenu : MonoBehaviour
{
    public GameObject menuPanel;
    public void ShowMenu()
    {
        menuPanel.SetActive(true);
    }

    public void HideMenu()
    {
        menuPanel.SetActive(false);
    }
}
