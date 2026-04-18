using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AbilityCooldownUI : MonoBehaviour
{
    public Button freezeButton;
    public TMP_Text cooldownText;

    void Update()
    {
        if (AbilityManager.Instance == null) return;

        bool ready = AbilityManager.Instance.FreezeReady;

        freezeButton.interactable = ready;

        cooldownText.text = ready
            ? "Freeze"
            : $"{AbilityManager.Instance.FreezeCooldownRemaining:F1}s";
    }
}