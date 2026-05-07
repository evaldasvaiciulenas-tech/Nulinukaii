using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayMenu()
    {
        SceneManager.LoadScene("PlayMenu");
    }

    public void GoToMainMenu()
    {
    SceneManager.LoadScene("MainMenu");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

        public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");
    }


}