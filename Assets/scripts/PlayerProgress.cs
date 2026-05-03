using UnityEngine;

/// <summary>
/// Singleton that persists across scenes and handles all save/load logic.
/// Stores: highest level unlocked, and best completion time per level.
/// Uses PlayerPrefs for simple, lightweight persistence.
///
/// Self-bootstrapping: created automatically at game start via
/// RuntimeInitializeOnLoadMethod — you do NOT need to place this in any scene.
/// </summary>
public class PlayerProgress : MonoBehaviour
{
    public static PlayerProgress Instance;

    public const int TOTAL_LEVELS = 10;

    /// <summary>
    /// The build index of your Level 1 scene.
    /// Example: if your Build Settings order is
    ///   0: MainMenu  1: ChooseDifficulty  2: ChooseLevel  3: Level1 ... 12: Level10
    /// then set this to 3.
    /// </summary>
    public const int FIRST_LEVEL_BUILD_INDEX = 4;

    // The highest level number the player may enter (1-based).
    // Starts at 1 so Level 1 is always accessible.
    private int highestUnlockedLevel = 1;

    // Best times per level in seconds. Index 0 unused; indices 1–10 map to levels.
    // 0 means the level has never been completed.
    private float[] bestTimes = new float[TOTAL_LEVELS + 1];

    // ── Bootstrap ────────────────────────────────────────────────────

    /// <summary>
    /// Called automatically by Unity before the first scene loads.
    /// Creates the singleton so it is available in every scene, including
    /// ChooseLevel, without any manual GameObject setup.
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;

        GameObject go = new GameObject("PlayerProgress");
        DontDestroyOnLoad(go);
        Instance = go.AddComponent<PlayerProgress>();
        Instance.Load();
    }

    void Awake()
    {
        // Guard against a second instance if someone manually adds this to a scene.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        // Normal path when Awake fires after Bootstrap has already run:
        // nothing extra needed — Bootstrap already called Load().
    }

    // ── Public queries ──────────────────────────────────────────────

    /// <summary>Returns true if the given level number (1-based) is unlocked.</summary>
    public bool IsLevelUnlocked(int levelNumber)
    {
        return levelNumber <= highestUnlockedLevel;
    }

    /// <summary>
    /// Returns the best completion time for a level in seconds.
    /// Returns 0 if the level has never been completed.
    /// </summary>
    public float GetBestTime(int levelNumber)
    {
        if (levelNumber < 1 || levelNumber > TOTAL_LEVELS) return 0f;
        return bestTimes[levelNumber];
    }

    // ── Called by GameManager on win ────────────────────────────────

    /// <summary>
    /// Called when the player wins a level.
    /// Updates best time and unlocks the next level if needed.
    /// </summary>
    public void RecordLevelComplete(int levelNumber, float completionTime)
    {
        if (levelNumber < 1 || levelNumber > TOTAL_LEVELS) return;

        // Update best time (keep lowest; first time always saves)
        if (bestTimes[levelNumber] == 0f || completionTime < bestTimes[levelNumber])
            bestTimes[levelNumber] = completionTime;

        // Unlock next level
        int nextLevel = levelNumber + 1;
        if (nextLevel <= TOTAL_LEVELS && nextLevel > highestUnlockedLevel)
            highestUnlockedLevel = nextLevel;

        Save();
    }

    // ── Persistence ─────────────────────────────────────────────────

    void Save()
    {
        PlayerPrefs.SetInt("HighestUnlockedLevel", highestUnlockedLevel);
        for (int i = 1; i <= TOTAL_LEVELS; i++)
            PlayerPrefs.SetFloat("BestTime_Level" + i, bestTimes[i]);
        PlayerPrefs.Save();
    }

    void Load()
    {
        highestUnlockedLevel = PlayerPrefs.GetInt("HighestUnlockedLevel", 1);
        for (int i = 1; i <= TOTAL_LEVELS; i++)
            bestTimes[i] = PlayerPrefs.GetFloat("BestTime_Level" + i, 0f);
    }

    /// <summary>
    /// Resets all progress. Useful for a "reset save" button in settings.
    /// </summary>
    public void ResetProgress()
    {
        highestUnlockedLevel = 1;
        bestTimes = new float[TOTAL_LEVELS + 1];
        Save();
    }

    // ── Utility ─────────────────────────────────────────────────────

    /// <summary>Formats a time in seconds as "m:ss.ff" (e.g. "1:23.45").</summary>
    public static string FormatTime(float seconds)
    {
        int minutes = (int)(seconds / 60);
        float secs = seconds % 60;
        return string.Format("{0}:{1:00.00}", minutes, secs);
    }
}
