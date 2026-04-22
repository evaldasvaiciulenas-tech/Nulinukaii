using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BoostButtonUI : MonoBehaviour
{
    public Button boostButton;
    public TMP_Text cooldownText;
    public GameObject selectionGlow;

    void Update()
    {
        if (AbilityManager.Instance == null) return;

        bool ready = AbilityManager.Instance.BoostReady;
        bool active = AbilityManager.Instance.IsBoostMode;

        boostButton.interactable = ready;
        if (selectionGlow != null) selectionGlow.SetActive(active);

        cooldownText.text = active ? "Select!" : ready ? "Boost" : $"{AbilityManager.Instance.BoostCooldownRemaining:F1}s";
    }

    public void HideButton() => gameObject.SetActive(false);
}