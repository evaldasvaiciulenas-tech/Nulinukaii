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

    public void OpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }
}