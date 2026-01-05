using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


// Class that handles populating the Mission Select Menu's UI/Buttons
public class MissionSelectUI : MonoBehaviour
{
    public Button[] missionButtons;
    public TextMeshProUGUI[] missionButtonLabels;
    // On start, populate the menu with the appropriate information
    void Start()
    {
        for (int i = 0; i < missionButtons.Length; i++)
        {
            Debug.Log($"Setting button {i}");
            int index = i;

            MissionInfo m = MissionManager.Instance.missions[i];

            missionButtonLabels[i].text = $"{m.difficulty}\n{m.roomCount} rooms\nSize {m.minSize}-{m.maxSize}";
            Debug.Log($"Button {i} text before: {missionButtonLabels[i].text}");
            
            missionButtons[i].onClick.AddListener(() =>
            {
                SelectMission(index);
            });
        }
    }

    void SelectMission(int i)
    {
        MissionInfo m = MissionManager.Instance.missions[i];

        SelectedMission.index = i;

        SceneManager.LoadScene("GameScene");
    }
}

public static class SelectedMission
{
    public static int index;
}
