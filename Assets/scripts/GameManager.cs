using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject winPanel; // UI panel (vėliau)
    public GameObject losePanel;

    public AudioClip winSound;
    public AudioClip loseSound;
    private AudioSource audioSource;
    private MusicManager musicManager;

    public GameObject pauseButton;
    public GameObject freezeButton;


    private bool gameEnded = false;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        musicManager = FindObjectOfType<MusicManager>();
    }

    void Update()
    {
        if (gameEnded) return;

        CheckWinCondition();
        CheckLoseCondition(); 
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
    void CheckLoseCondition()
    {
        NodePastatas[] nodes = FindObjectsOfType<NodePastatas>();

        foreach (NodePastatas node in nodes)
        {
            if (node.owner == NodePastatas.OwnerType.Player)
                return; // dar turi bent vieną → nepralaimėjai
        }

        LoseGame();
    }

    void WinGame()
    {
        gameEnded = true;

        Debug.Log("WIN!");

        if (audioSource != null && winSound != null)
            audioSource.PlayOneShot(winSound);

        if (musicManager != null)
            musicManager.StopMusic();

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;

        if (pauseButton != null)
            pauseButton.SetActive(false);
        if (freezeButton != null)
            freezeButton.SetActive(false);
    }
    void LoseGame()
    {
        gameEnded = true;

        Debug.Log("LOSE!");

        if (audioSource != null && loseSound != null)
            audioSource.PlayOneShot(loseSound);

        if (musicManager != null)
            musicManager.StopMusic();

        if (losePanel != null)
            losePanel.SetActive(true);

        Time.timeScale = 0f;

        if (pauseButton != null)
            pauseButton.SetActive(false);
        if (freezeButton != null)
            freezeButton.SetActive(false);
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }

}