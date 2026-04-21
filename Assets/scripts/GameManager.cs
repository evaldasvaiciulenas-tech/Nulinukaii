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
        // Reset static state
        NodePastatas.playerActiveLines = 0;
        NodePastatas.aiActiveLines = 0;

        ApplyDifficulty();

        audioSource = GetComponent<AudioSource>();
        musicManager = FindObjectOfType<MusicManager>();
    }

    void ApplyDifficulty()
    {
        if (DifficultyManager.Instance == null) return;

        float generateInterval, freezeCooldown, freezeDuration;
        float aggression, actionInterval; // add these
        int maxLines;

        switch (DifficultyManager.Instance.currentDifficulty)
        {
            case DifficultyManager.Difficulty.Easy:
                generateInterval = 1f;
                maxLines = 2;
                freezeCooldown = 8f;
                freezeDuration = 5f;
                aggression = 0.3f;    
                actionInterval = 2; 
                break;
            case DifficultyManager.Difficulty.Hard:
                generateInterval = 0.7f;
                maxLines = 4;
                freezeCooldown = 15f;
                freezeDuration = 5f;
                aggression = 0.8f;    // almost always attacks
                actionInterval = 1.5f;  // thinks faster
                break;
            default: // Normal
                generateInterval = 1f;
                maxLines = 3;
                freezeCooldown = 10f;
                freezeDuration = 5f;
                aggression = 0.6f;    // original value
                actionInterval = 2f;  // original value
                break;
        }

        foreach (NodePastatas node in FindObjectsOfType<NodePastatas>())
        {
            if (node.owner == NodePastatas.OwnerType.AI)
            {
                node.generateInterval = generateInterval;
                node.maxActiveLines = maxLines;
            }
        }

        // Apply to AIBot
        AIBot bot = FindObjectOfType<AIBot>();
        if (bot != null)
        {
            bot.aggression = aggression;
            bot.actionInterval = actionInterval;
        }

        if (AbilityManager.Instance != null)
        {
            AbilityManager.Instance.freezeCooldown = freezeCooldown;
            AbilityManager.Instance.freezeDuration = freezeDuration;
        }
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