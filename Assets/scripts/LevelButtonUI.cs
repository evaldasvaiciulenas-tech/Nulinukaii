using UnityEngine;
using UnityEngine.UI;   // swap for TMPro if needed

/// <summary>
/// Attach this to each level-select button in your ChooseLevel scene.
/// Set the levelNumber field in the Inspector (1–10) for each button.
///
/// The script will:
///  - Disable the button if the level is locked
///  - Show a lock icon (optional) when locked
///  - Show the player's best time below the button label when unlocked & completed
/// </summary>
public class LevelButtonUI : MonoBehaviour
{
    [Header("Level")]
    [Tooltip("1-based level number this button represents.")]
    public int levelNumber = 1;

    [Tooltip("Exact scene name to load for this level.")]
    public string sceneName = "Level1";

    [Header("UI References")]
    public Button button;                    // the Button component on this GameObject
    public Text bestTimeText;               // optional Text below the level number; swap to TMP_Text if needed
    public GameObject lockIcon;             // optional lock icon shown when level is locked

    void Start()
    {
        Refresh();
    }

    /// <summary>Call this to update the button's visual state from saved progress.</summary>
    public void Refresh()
    {
        bool unlocked = PlayerProgress.Instance != null
                        && PlayerProgress.Instance.IsLevelUnlocked(levelNumber);

        // Fallback: if PlayerProgress isn't in the scene yet, keep everything unlocked
        // (so the game still works in the editor without the singleton present)
        if (PlayerProgress.Instance == null)
            unlocked = true;

        // Enable / disable the button
        if (button != null)
            button.interactable = unlocked;

        // Show or hide the lock icon
        if (lockIcon != null)
            lockIcon.SetActive(!unlocked);

        // Show best time if completed
        if (bestTimeText != null)
        {
            float best = PlayerProgress.Instance != null
                         ? PlayerProgress.Instance.GetBestTime(levelNumber)
                         : 0f;

            if (best > 0f)
                bestTimeText.text = "Best: " + PlayerProgress.FormatTime(best);
            else if (unlocked)
                bestTimeText.text = "Not completed";
            else
                bestTimeText.text = "";
        }
    }

    /// <summary>Hook this up to the button's OnClick event in the Inspector.</summary>
    public void OnClick()
    {
        if (PlayerProgress.Instance != null && !PlayerProgress.Instance.IsLevelUnlocked(levelNumber))
            return; // safety guard — button should already be non-interactable

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
