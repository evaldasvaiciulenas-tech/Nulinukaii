using UnityEngine;
using UnityEngine.SceneManagement;

public class ChooseDifficulty : MonoBehaviour
{
    public void SetEasy()
    {
        DifficultyManager.Instance.currentDifficulty = DifficultyManager.Difficulty.Easy;
        SceneManager.LoadScene("ChooseLevel");
    }

    public void SetNormal()
    {
        DifficultyManager.Instance.currentDifficulty = DifficultyManager.Difficulty.Normal;
        SceneManager.LoadScene("ChooseLevel");
    }

    public void SetHard()
    {
        DifficultyManager.Instance.currentDifficulty = DifficultyManager.Difficulty.Hard;
        SceneManager.LoadScene("ChooseLevel");
    }

    public void Back()
    {
        SceneManager.LoadScene("MainMenu");
    }
}