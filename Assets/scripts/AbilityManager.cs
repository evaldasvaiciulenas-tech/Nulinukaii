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

    [Header("Speed Settings")]
    public float speedDuration = 5f;
    public float speedCooldown = 12f;
    public float speedMultiplier = 2f;

    [Header("Shield Settings")]
    public float shieldDuration = 5f;
    public float shieldCooldown = 15f;

    [Header("Ability Visual Effects")]
    public GameObject sabotageEffectPrefab;
    public GameObject freezeEffectPrefab;
    public GameObject boostEffectPrefab;
    public GameObject speedEffectPrefab;
    public GameObject shieldEffectPrefab;

    private bool freezeMode = false;
    private float freezeCooldownTimer = 0f;

    private bool sabotageMode = false;
    private float sabotageCooldownTimer = 0f;

    private bool boostMode = false;
    private float boostCooldownTimer = 0f;

    private bool speedMode = false;
    private float speedCooldownTimer = 0f;
    private float speedActiveTimer = 0f;

    private bool shieldMode = false;
    private float shieldCooldownTimer = 0f;

    public bool IsFreezeMode => freezeMode;
    public bool FreezeReady => freezeCooldownTimer <= 0f;
    public float FreezeCooldownRemaining => freezeCooldownTimer;

    public bool IsSabotageMode => sabotageMode;
    public bool SabotageReady => sabotageCooldownTimer <= 0f;
    public float SabotageCooldownRemaining => sabotageCooldownTimer;

    public bool IsBoostMode => boostMode;
    public bool BoostReady => boostCooldownTimer <= 0f;
    public float BoostCooldownRemaining => boostCooldownTimer;

    public bool IsSpeedMode => speedMode;
    public bool SpeedReady => speedCooldownTimer <= 0f;
    public bool IsSpeedActive => speedActiveTimer > 0f;
    public float SpeedCooldownRemaining => speedCooldownTimer;

    public bool IsShieldMode => shieldMode;
    public bool ShieldReady => shieldCooldownTimer <= 0f;
    public float ShieldCooldownRemaining => shieldCooldownTimer;

    void Awake()
    {
        Instance = this;
        freezeCooldownTimer = freezeCooldown;
        sabotageCooldownTimer = sabotageCooldown;
        boostCooldownTimer = boostCooldown;
        speedCooldownTimer = speedCooldown;
        shieldCooldownTimer = shieldCooldown;
    }

    void Update()
    {
        if (freezeCooldownTimer > 0f) freezeCooldownTimer -= Time.deltaTime;
        if (sabotageCooldownTimer > 0f) sabotageCooldownTimer -= Time.deltaTime;
        if (boostCooldownTimer > 0f) boostCooldownTimer -= Time.deltaTime;
        if (speedCooldownTimer > 0f) speedCooldownTimer -= Time.deltaTime;
        if (speedActiveTimer > 0f) speedActiveTimer -= Time.deltaTime;
        if (shieldCooldownTimer > 0f) shieldCooldownTimer -= Time.deltaTime;

        if (freezeMode) HandleFreezeClick();
        else if (sabotageMode) HandleSabotageClick();
        else if (boostMode) HandleBoostClick();
        else if (shieldMode) HandleShieldClick();
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
                SpawnEffect(
                    freezeEffectPrefab,
                    node.transform.position
                );
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

                SpawnEffect(
                    sabotageEffectPrefab,
                    node.transform.position
                );
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
                    SpawnEffect(
                        boostEffectPrefab,
                        node.transform.position
                    );
                boostMode = false;
                boostCooldownTimer = boostCooldown;
                Debug.Log("Boost used on: " + node.name);
                break;
            }
        }
    }

    void HandleShieldClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        foreach (Collider2D hit in Physics2D.OverlapCircleAll(mousePos, 0.5f))
        {
            NodePastatas node = hit.GetComponent<NodePastatas>();
            if (node != null && node.owner == NodePastatas.OwnerType.Player)
            {
                node.ShieldNode(shieldDuration);
                SpawnEffect(
                    shieldEffectPrefab,
                    node.transform.position
                );
                shieldMode = false;
                shieldCooldownTimer = shieldCooldown;
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

    public void ActivateSpeed()
    {
        if (speedCooldownTimer > 0f) return;
        speedCooldownTimer = speedCooldown;
        speedActiveTimer = speedDuration;

        foreach (Student s in FindObjectsOfType<Student>())
            if (s.GetSource() != null && s.GetSource().owner == NodePastatas.OwnerType.Player)
                s.ApplySpeedBoost(speedMultiplier, speedDuration);

        NodePastatas[] playerNodes = FindObjectsOfType<NodePastatas>();

        foreach (NodePastatas node in playerNodes)
        {
            if (node.owner == NodePastatas.OwnerType.Player)
            {
                SpawnEffect(
                    speedEffectPrefab,
                    node.transform.position
                );
            }
        }
        StartCoroutine(SpeedAllNodesCoroutine());
    }

    private System.Collections.IEnumerator SpeedAllNodesCoroutine()
    {
        NodePastatas[] playerNodes = System.Array.FindAll(
            FindObjectsOfType<NodePastatas>(),
            n => n.owner == NodePastatas.OwnerType.Player
        );

        foreach (NodePastatas node in playerNodes)
            node.BoostSendSpeed(speedMultiplier, speedDuration);

        yield return null;
    }

    public void ActivateShield()
    {
        if (shieldCooldownTimer > 0f) return;
        shieldMode = true; freezeMode = false; sabotageMode = false; boostMode = false; speedMode = false;
    }
    void SpawnEffect(GameObject effectPrefab, Vector3 position)
    {
        if (effectPrefab == null) return;

        Instantiate(
            effectPrefab,
            position,
            Quaternion.identity
        );
    }
}