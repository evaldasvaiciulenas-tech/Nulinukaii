using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShieldButtonUI : MonoBehaviour
{
    public Button shieldButton;
    public TMP_Text cooldownText;
    public GameObject selectionGlow;

    void Update()
    {
        if (AbilityManager.Instance == null) return;

        bool ready = AbilityManager.Instance.ShieldReady;
        bool active = AbilityManager.Instance.IsShieldMode;

        shieldButton.interactable = ready;
        if (selectionGlow != null) selectionGlow.SetActive(active);

        cooldownText.text = active ? "Select!" : ready ? "Shield" : $"{AbilityManager.Instance.ShieldCooldownRemaining:F1}s";
    }

    public void HideButton() => gameObject.SetActive(false);
}