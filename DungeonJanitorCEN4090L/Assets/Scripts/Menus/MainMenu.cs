using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
       
        SaveSystem.LoadOnStart = false;
        Time.timeScale = 1f;
        EscToggleOptions.GameIsPaused = false;  
        SceneManager.LoadSceneAsync(1);
    }

    public void ContinueGame()
    {
        if (SaveSystem.SaveExists())
        {
            SaveSystem.LoadOnStart = true;       
            Time.timeScale = 1f;
            EscToggleOptions.GameIsPaused = false;
            SceneManager.LoadSceneAsync(1);
        }
        else
        {
            Debug.Log("Continue pressed, but no save file found.");
        }
    }

    public void MenuScreen()
    {
       
        SaveSystem.LoadOnStart = false;         
        EscToggleOptions.GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void OptionsScreen()
    {
        SceneManager.LoadSceneAsync(2);
    }
}
