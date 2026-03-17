using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winPanel; // UI panel (vėliau)

    private bool gameEnded = false;

    void Update()
    {
        if (gameEnded) return;

        CheckWinCondition();
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f; // svarbu (kad po win veiktu)
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void CheckWinCondition()
    {
        NodePastatas[] nodes = FindObjectsOfType<NodePastatas>();

        foreach (NodePastatas node in nodes)
        {
            if (node.owner != NodePastatas.OwnerType.Player)
                return; // jei bent vienas ne player - dar nelaimėta
        }

        WinGame();
    }

    void WinGame()
    {
        gameEnded = true;

        Debug.Log("WIN!");

        Time.timeScale = 0f; // sustabdo žaidimą

        if (winPanel != null)
            winPanel.SetActive(true);
    }
}