using UnityEngine;
using UnityEngine.SceneManagement;

public class EscToggleOptions : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuPanel;

    public static bool GameIsPaused { get; set; }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        GameIsPaused = true;
        pauseMenuPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        GameIsPaused = false;
        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    
    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("MainMenu");
    }
}
