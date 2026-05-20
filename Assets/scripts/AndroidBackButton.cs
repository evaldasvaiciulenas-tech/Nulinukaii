using UnityEngine;
using UnityEngine.SceneManagement;

public class AndroidBackButton : MonoBehaviour
{
    public enum BackAction { LoadScene, Quit, Resume }
    public BackAction action = BackAction.LoadScene;
    public string targetScene = "MainMenu";

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (action == BackAction.Quit)
                Application.Quit();
            else if (action == BackAction.Resume)
                Time.timeScale = 1f;
            else
                SceneManager.LoadScene(targetScene);
        }
    }
}