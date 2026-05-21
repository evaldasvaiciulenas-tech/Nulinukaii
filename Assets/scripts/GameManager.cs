using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("Timer")]
    public TMP_Text timerText;
    public TMP_Text winTimeText;
    public TMP_Text winStarsText;

    [Header("Countdown")]
    public TMP_Text countdownText;

    private bool gameEnded = false;
    private bool countdownDone = false;
    private float elapsedTime = 0f;
    private int levelNumber = -1;

    void Start()
    {
        Application.targetFrameRate = 60;
        NodePastatas.playerActiveLines = 0;
        NodePastatas.aiActiveLines = 0;

        int buildIndex = SceneManager.GetActiveScene().buildIndex;
        levelNumber = buildIndex - PlayerProgress.FIRST_LEVEL_BUILD_INDEX + 1;

        ApplyDifficulty();

        audioSource = GetComponent<AudioSource>();
        musicManager = FindObjectOfType<MusicManager>();

        if (musicManager != null)
            musicManager.PlayMusic();

        StartCoroutine(StartCountdown());
    }

    System.Collections.IEnumerator StartCountdown()
    {
        Time.timeScale = 0f;
        if (countdownText != null) countdownText.gameObject.SetActive(true);

        if (countdownText != null) countdownText.text = "3";
        yield return new WaitForSecondsRealtime(1f);
        if (countdownText != null) countdownText.text = "2";
        yield return new WaitForSecondsRealtime(1f);
        if (countdownText != null) countdownText.text = "1";
        yield return new WaitForSecondsRealtime(1f);
        if (countdownText != null) countdownText.text = "GO!";
        yield return new WaitForSecondsRealtime(0.5f);

        if (countdownText != null) countdownText.gameObject.SetActive(false);
        Time.timeScale = 1f;
        countdownDone = true;
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
            default:
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
        if (gameEnded || !countdownDone) return;

        elapsedTime += Time.deltaTime;

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
        Handheld.Vibrate();

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        if (PlayerProgress.Instance != null && levelNumber >= 1)
            PlayerProgress.Instance.RecordLevelComplete(levelNumber, elapsedTime);

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

        winPanel.GetComponentInChildren<ConfettiEffect>().enabled = true;

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
        Handheld.Vibrate();

        if (timerText != null)
            timerText.gameObject.SetActive(false);

        Debug.Log("LOSE!");

        if (audioSource != null && loseSound != null)
            audioSource.PlayOneShot(loseSound);

        if (musicManager != null)
            musicManager.StopMusic();

        if (losePanel != null)
        {
            losePanel.SetActive(true);
            losePanel.GetComponentInChildren<LoseConfettiEffect>().enabled = true;
        }

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