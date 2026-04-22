using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SabotageButtonUI : MonoBehaviour
{
    public Button sabotageButton;
    public TMP_Text cooldownText;
    public GameObject selectionGlow;

    void Update()
    {
        if (AbilityManager.Instance == null) return;

        bool ready = AbilityManager.Instance.SabotageReady;
        bool active = AbilityManager.Instance.IsSabotageMode;

        sabotageButton.interactable = ready;
        if (selectionGlow != null) selectionGlow.SetActive(active);

        cooldownText.text = active ? "Select!" : ready ? "Sabotage" : $"{AbilityManager.Instance.SabotageCooldownRemaining:F1}s";
    }

    public void HideButton() => gameObject.SetActive(false);
}