using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    [Header("Freeze Settings")]
    public float freezeDuration = 5f;
    public float freezeCooldown = 10f;

    [Header("Sabotage Settings")]
    public float sabotageCooldown = 12f;

    [Header("Boost Settings")]
    public float boostDuration = 5f;
    public float boostCooldown = 12f;
    public float boostMultiplier = 0.5f; // 50% faster = interval * 0.5

    private bool freezeMode = false;
    private float freezeCooldownTimer = 0f;

    private bool sabotageMode = false;
    private float sabotageCooldownTimer = 0f;

    private bool boostMode = false;
    private float boostCooldownTimer = 0f;

    public bool IsFreezeMode => freezeMode;
    public bool FreezeReady => freezeCooldownTimer <= 0f;
    public float FreezeCooldownRemaining => freezeCooldownTimer;

    public bool IsSabotageMode => sabotageMode;
    public bool SabotageReady => sabotageCooldownTimer <= 0f;
    public float SabotageCooldownRemaining => sabotageCooldownTimer;

    public bool IsBoostMode => boostMode;
    public bool BoostReady => boostCooldownTimer <= 0f;
    public float BoostCooldownRemaining => boostCooldownTimer;

    void Awake()
    {
        Instance = this;
        freezeCooldownTimer = freezeCooldown;
        sabotageCooldownTimer = sabotageCooldown;
        boostCooldownTimer = boostCooldown;
    }

    void Update()
    {
        if (freezeCooldownTimer > 0f) freezeCooldownTimer -= Time.deltaTime;
        if (sabotageCooldownTimer > 0f) sabotageCooldownTimer -= Time.deltaTime;
        if (boostCooldownTimer > 0f) boostCooldownTimer -= Time.deltaTime;

        if (freezeMode) HandleFreezeClick();
        else if (sabotageMode) HandleSabotageClick();
        else if (boostMode) HandleBoostClick();
    }

    void HandleFreezeClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(mousePos, 0.5f))
        {
            NodePastatas node = hit.GetComponent<NodePastatas>();
            if (node != null && node.owner != NodePastatas.OwnerType.Player)
            {
                node.FreezeNode(freezeDuration);
                freezeMode = false;
                freezeCooldownTimer = freezeCooldown;
                break;
            }
        }
    }

    void HandleSabotageClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(mousePos, 0.5f))
        {
            NodePastatas node = hit.GetComponent<NodePastatas>();
            if (node != null && node.owner == NodePastatas.OwnerType.AI)
            {
                node.studentCount = Mathf.FloorToInt(node.studentCount * 0.5f);
                sabotageMode = false;
                sabotageCooldownTimer = sabotageCooldown;
                Debug.Log("Sabotage used on: " + node.name);
                break;
            }
        }
    }

    void HandleBoostClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(mousePos, 0.5f))
        {
            NodePastatas node = hit.GetComponent<NodePastatas>();
            if (node != null && node.owner == NodePastatas.OwnerType.Player)
            {
                node.BoostNode(boostDuration, boostMultiplier);
                boostMode = false;
                boostCooldownTimer = boostCooldown;
                Debug.Log("Boost used on: " + node.name);
                break;
            }
        }
    }

    public void ActivateFreeze()
    {
        if (freezeCooldownTimer > 0f) return;
        freezeMode = true; sabotageMode = false; boostMode = false;
    }

    public void ActivateSabotage()
    {
        if (sabotageCooldownTimer > 0f) return;
        sabotageMode = true; freezeMode = false; boostMode = false;
    }

    public void ActivateBoost()
    {
        if (boostCooldownTimer > 0f) return;
        boostMode = true; freezeMode = false; sabotageMode = false;
    }
}