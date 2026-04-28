using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedButtonUI : MonoBehaviour
{
    public Button speedButton;
    public TMP_Text cooldownText;
    public GameObject selectionGlow;

    void Update()
    {
        if (AbilityManager.Instance == null) return;

        bool ready = AbilityManager.Instance.SpeedReady;
        bool active = AbilityManager.Instance.IsSpeedMode;

        speedButton.interactable = ready;
        if (selectionGlow != null) selectionGlow.SetActive(active);

        cooldownText.text = active ? "Select!" : ready ? "Speed" : $"{AbilityManager.Instance.SpeedCooldownRemaining:F1}s";
    }

    public void HideButton() => gameObject.SetActive(false);
}