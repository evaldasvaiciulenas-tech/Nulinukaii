using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   // needed for Text component; swap for TMPro if you use TextMeshPro
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;


    public AudioClip winSound;
    public AudioClip loseSound;
    private AudioSource audioSource;
    private MusicManager musicManager;

    public GameObject pauseButton;
    public GameObject freezeButton;
    public GameObject sabotageButton;
    public GameObject boostButton;
    public GameObject speedButton;
    public GameObject shieldButton;

    // ── Timer UI ────────────────────────────────────────────────────
    // Assign a UI Text (or TMP_Text) in the Inspector to show elapsed time while playing.
    // Leave empty if you don't want an in-game timer display.
    [Header("Timer")]
    public TMP_Text timerText;
    public TMP_Text winTimeText;           
    public TMP_Text winStarsText;

    // ── Internals ───────────────────────────────────────────────────
    private bool gameEnded = false;
    private float elapsedTime = 0f;
    private int levelNumber = -1;        // 1-based level number, derived from build index

    void Start()
    {
        NodePastatas.playerActiveLines = 0;
        NodePastatas.aiActiveLines = 0;

        // Derive the 1-based level number from the scene's build index.
        // This assumes your level scenes occupy build indices starting at
        // PlayerProgress.FIRST_LEVEL_BUILD_INDEX. E.g. if Level 1 is build index 1,
        // Level 2 is build index 2, etc.
        // Derive which level this is (1-based) from the scene's build index.
        // Matches the FIRST_LEVEL_BUILD_INDEX constant in PlayerProgress —
        // update that one constant if your build order ever changes.
        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        levelNumber = buildIndex - PlayerProgress.FIRST_LEVEL_BUILD_INDEX + 1;

        ApplyDifficulty();

        audioSource = GetComponent<AudioSource>();
        musicManager = FindObjectOfType<MusicManager>();

        if (musicManager != null)
            musicManager.PlayMusic();
    }

    void ApplyDifficulty()
    {
        if (DifficultyManager.Instance == null) return;

        float intervalMultiplier;
        int maxLinesBonus;
        float freezeCooldown, freezeDuration;
        float aggression, actionInterval;

        switch (DifficultyManager.Instance.currentDifficulty)
        {
            case DifficultyManager.Difficulty.Easy:
                intervalMultiplier = 1.2f;
                maxLinesBonus = 0;
                freezeCooldown = 8f;
                freezeDuration = 5f;
                aggression = 0.4f;
                actionInterval = 2f;
                break;
            case DifficultyManager.Difficulty.Hard:
                intervalMultiplier = 0.7f;
                maxLinesBonus = 1;
                freezeCooldown = 15f;
                freezeDuration = 5f;
                aggression = 0.8f;
                actionInterval = 1.5f;
                break;
            default: // Normal
                intervalMultiplier = 1f;
                maxLinesBonus = 0;
                freezeCooldown = 10f;
                freezeDuration = 5f;
                aggression = 0.6f;
                actionInterval = 2f;
                break;
        }

        foreach (NodePastatas node in FindObjectsOfType<NodePastatas>())
        {
            if (node.owner == NodePastatas.OwnerType.AI)
            {
                node.generateInterval = node.baseGenerateInterval * intervalMultiplier;
                node.maxActiveLines = Mathf.Max(1, node.baseMaxActiveLines + maxLinesBonus);
            }
        }

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

        // Advance timer (Time.deltaTime is 0 when paused via Time.timeScale = 0)
        elapsedTime += Time.deltaTime;

        // Update the in-game timer display
        if (timerText != null)
            timerText.text = PlayerProgress.FormatTime(elapsedTime);

        CheckWinCondition();
        CheckLoseCondition();
    }

    public void ResetLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void CheckWinCondition()
    {
        NodePastatas[] nodes = FindObjectsOfType<NodePastatas>();
        foreach (NodePastatas node in nodes)
        {
            if (node.owner != NodePastatas.OwnerType.Player)
                return;
        }
        WinGame();
    }

    void CheckLoseCondition()
    {
        NodePastatas[] nodes = FindObjectsOfType<NodePastatas>();
        foreach (NodePastatas node in nodes)
        {
            if (node.owner == NodePastatas.OwnerType.Player)
                return;
        }
        LoseGame();
    }

    void WinGame()
    {
        gameEnded = true;

        // ── Save progress ──
        if (PlayerProgress.Instance != null && levelNumber >= 1)
        {
            PlayerProgress.Instance.RecordLevelComplete(levelNumber, elapsedTime);
        }

        // ── Show final time on win panel ──
        if (winTimeText != null)
            winTimeText.text = "Time: " + PlayerProgress.FormatTime(elapsedTime);

        if (winStarsText != null)
        {
            int stars = PlayerProgress.GetStarRating(levelNumber, elapsedTime);
            winStarsText.text = new string('★', stars) + new string('☆', 3 - stars);
        }

        Debug.Log("WIN! Time: " + PlayerProgress.FormatTime(elapsedTime));

        if (audioSource != null && winSound != null)
            audioSource.PlayOneShot(winSound);

        if (musicManager != null)
            musicManager.StopMusic();

        if (winPanel != null)
            winPanel.SetActive(true);

        Time.timeScale = 0f;

        if (pauseButton != null) pauseButton.SetActive(false);
        if (freezeButton != null) freezeButton.SetActive(false);
        if (sabotageButton != null) sabotageButton.SetActive(false);
        if (boostButton != null) boostButton.SetActive(false);
        if (speedButton != null) speedButton.SetActive(false);
        if (shieldButton != null) shieldButton.SetActive(false);
    }

    void LoseGame()
    {
        gameEnded = true;

        // Note: we do NOT save time on a loss — only wins count.

        Debug.Log("LOSE!");

        if (audioSource != null && loseSound != null)
            audioSource.PlayOneShot(loseSound);

        if (musicManager != null)
            musicManager.StopMusic();

        if (losePanel != null)
            losePanel.SetActive(true);

        Time.timeScale = 0f;

        if (pauseButton != null) pauseButton.SetActive(false);
        if (freezeButton != null) freezeButton.SetActive(false);
        if (sabotageButton != null) sabotageButton.SetActive(false);
        if (boostButton != null) boostButton.SetActive(false);
        if (speedButton != null) speedButton.SetActive(false);
        if (shieldButton != null) shieldButton.SetActive(false);
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex + 1);
    }
}