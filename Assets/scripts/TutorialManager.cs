using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Tutorial Manager – guides the player through core mechanics step by step.
/// </summary>
public class TutorialManager : MonoBehaviour
{
    // ── UI References ────────────────────────────────────────────────
    [Header("UI")]
    public GameObject tutorialPanel;
    public TMP_Text tutorialText;

    // ── Scene Node References ────────────────────────────────────────
    [Header("Nodes")]
    public NodePastatas playerNodeRef;
    public NodePastatas neutralNodeRef;
    public NodePastatas enemyNodeRef;

    // ── Internals ────────────────────────────────────────────────────
    private int currentStep = 0;

    // Tutorial texts
    private static readonly string[] StepTexts = new[]
    {
        // 0 – Welcome
        "Welcome to UniWar!\n\n" +
        "Your goal is to capture all NEUTRAL and ENEMY buildings\n\n" +
        "Tap to begin",

        // 1 – Select your node
        "STEP 1 – Select your building\n\n" +
        "Click on the KTU building\n\n" +
        "Buildings display the number of students",

        // 2 – Send students to a neutral
        "STEP 2 – Send students\n\n" +
        "Hold your finger / mouse on your building\n\n" +
        "and DRAG towards the BLACK (neutral) building.\n\n" +
        "Release – the students will move!\n\n" +
        "Your faculty must have at least 5 students",

        // 3 – Capture neutral
        "STEP 3 – Capture\n\n" +
        "Be the first to capture neutral buildings!",

        // 4 – Attack enemy
        "STEP 4 – Attack\n\n" +
        "Now drag from your building toward the\n\n" +
        "RED (enemy) building\n\n" +
        "Capture it!",

        // 5 – Special ability
        "STEP 5 – Special abilities\n\n" +
        "The button in the bottom right is your special ability\n\n" +
        "Sabotage – reduces the enemy’s student count\n\n" +
        "Press the Sabotage button and click on the enemy building!\n\n" +
        "You will get different abilities in different levels",

        // 6 – Finished
        "Tutorial complete!\n\n" +
        "Now you know the basic mechanics\n\n" +
        "Capture all buildings and win!\n\n" +
        "Good luck!"
    };

    // ── Unity Lifecycle ──────────────────────────────────────────────

    void Start()
    {
        ShowStep(0);
        StartCoroutine(TutorialFlow());
    }

    // ── Main Flow ─────────────────────────────────────────────────────

    IEnumerator TutorialFlow()
    {
        // STEP 0 – Welcome
        currentStep = 0;
        ShowStep(0);
        yield return WaitForClick();

        // STEP 1 – Select player node
        currentStep = 1;
        ShowStep(1);
        yield return WaitUntilPlayerSelectsNode(playerNodeRef);

        // STEP 2 – Send to neutral
        currentStep = 2;
        ShowStep(2);
        yield return WaitUntilStudentsSent(playerNodeRef, neutralNodeRef);

        // STEP 3 – Wait capture
        currentStep = 3;
        ShowStep(3);
        yield return WaitUntilNodeCaptured(neutralNodeRef, NodePastatas.OwnerType.Player);

        // STEP 4 – Attack enemy
        currentStep = 4;
        ShowStep(4);
        yield return WaitUntilStudentsSent(playerNodeRef, enemyNodeRef);

        // STEP 5 – Ability
        currentStep = 5;
        ShowStep(5);
        yield return WaitUntilAbilityUsed();

        // STEP 6 – Finish
        currentStep = 6;
        ShowStep(6);
        yield return WaitForClick();

        EndTutorial();
    }

    // ── UI ───────────────────────────────────────────────────────────

    void ShowStep(int step)
    {
        if (tutorialPanel != null)
            tutorialPanel.SetActive(true);

        if (tutorialText != null)
            tutorialText.text = StepTexts[Mathf.Clamp(step, 0, StepTexts.Length - 1)];
    }

    // ── Wait Helpers ─────────────────────────────────────────────────

    /// Waits for a clean click (prevents skipping multiple steps)
    IEnumerator WaitForClick()
    {
        // Wait until current click is released
        while (Input.GetMouseButton(0))
            yield return null;

        // Wait for a new click
        while (!Input.GetMouseButtonDown(0))
            yield return null;
    }

    /// Wait until player clicks specific node
    IEnumerator WaitUntilPlayerSelectsNode(NodePastatas node)
    {
        if (node == null) yield break;

        while (true)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D[] hits = Physics2D.OverlapCircleAll(mousePos, 0.5f);

                foreach (Collider2D hit in hits)
                {
                    if (hit.gameObject == node.gameObject)
                        yield break;
                }
            }
            yield return null;
        }
    }

    /// Wait until students are sent (source loses students)
    IEnumerator WaitUntilStudentsSent(NodePastatas source, NodePastatas target)
    {
        if (source == null || target == null) yield break;

        int before = source.studentCount;

        while (source.studentCount >= before || source.studentCount < 1)
        {
            before = Mathf.Max(before, source.studentCount);
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
    }

    /// Wait until node is captured
    IEnumerator WaitUntilNodeCaptured(NodePastatas node, NodePastatas.OwnerType desiredOwner)
    {
        if (node == null) yield break;

        while (node.owner != desiredOwner)
            yield return new WaitForSeconds(0.25f);
    }

    /// Wait until any ability is used
    IEnumerator WaitUntilAbilityUsed()
    {
        if (AbilityManager.Instance == null)
        {
            yield return new WaitForSeconds(3f);
            yield break;
        }

        float freezeBefore   = AbilityManager.Instance.FreezeCooldownRemaining;
        float sabotageBefore = AbilityManager.Instance.SabotageCooldownRemaining;
        float boostBefore    = AbilityManager.Instance.BoostCooldownRemaining;

        while (true)
        {
            bool freezeUsed   = AbilityManager.Instance.FreezeCooldownRemaining   > freezeBefore + 1f;
            bool sabotageUsed = AbilityManager.Instance.SabotageCooldownRemaining > sabotageBefore + 1f;
            bool boostUsed    = AbilityManager.Instance.BoostCooldownRemaining    > boostBefore + 1f;

            if (freezeUsed || sabotageUsed || boostUsed)
                yield break;

            yield return new WaitForSeconds(0.1f);
        }
    }

    // ── End ──────────────────────────────────────────────────────────
    void LateUpdate()
    {
        if (currentStep == 6 && tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }
    }
    void EndTutorial()
    {
        StopAllCoroutines();

        if (tutorialPanel != null)
            tutorialPanel.SetActive(false);

        PlayerPrefs.SetInt("TutorialComplete", 1);
        PlayerPrefs.Save();

        Debug.Log("[Tutorial] Completed!");
    }
}