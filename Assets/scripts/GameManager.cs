using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winPanel; // UI panel (vėliau)

    public AudioClip winSound;
    private AudioSource audioSource;

    private bool gameEnded = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

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

        if (audioSource != null && winSound != null)
            audioSource.PlayOneShot(winSound);

        Time.timeScale = 0f; // sustabdo žaidimą

        if (winPanel != null)
            winPanel.SetActive(true);
    }
}