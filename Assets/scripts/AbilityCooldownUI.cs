using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FreezeButtonUI : MonoBehaviour
{
    public Button freezeButton;
    public TMP_Text cooldownText;
    public GameObject selectionGlow; // drag SelectionGlow here

    void Update()
    {
        if (AbilityManager.Instance == null) return;

        bool ready = AbilityManager.Instance.FreezeReady;
        bool active = AbilityManager.Instance.IsFreezeMode;

        freezeButton.interactable = ready;

        // show/hide the glow based on whether freeze is awaiting a target
        if (selectionGlow != null)
            selectionGlow.SetActive(active);

        cooldownText.text = active
            ? "Select!"
            : ready
                ? "Freeze"
                : $"{AbilityManager.Instance.FreezeCooldownRemaining:F1}s";
    }

    public void HideButton()
    {
        gameObject.SetActive(false);
    }
}