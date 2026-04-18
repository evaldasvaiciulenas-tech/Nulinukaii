using UnityEngine;

public class AbilityManager : MonoBehaviour
{
    public static AbilityManager Instance;

    [Header("Freeze Settings")]
    public float freezeDuration = 5f;
    public float freezeCooldown = 10f;

    private bool freezeMode = false;
    private float freezeCooldownTimer = 0f;

    public bool IsFreezeMode => freezeMode;
    public bool FreezeReady => freezeCooldownTimer <= 0f;
    public float FreezeCooldownRemaining => freezeCooldownTimer;

    void Awake()
    {
        Instance = this;
        freezeCooldownTimer = freezeCooldown;
    }

    void Update()
    {
        if (freezeCooldownTimer > 0f)
            freezeCooldownTimer -= Time.deltaTime;

        if (!freezeMode) return;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D hit = Physics2D.OverlapCircle(mousePos, 0.5f);

            if (hit != null)
            {
                NodePastatas node = hit.GetComponent<NodePastatas>();

                if (node != null && node.owner != NodePastatas.OwnerType.Player)
                {
                    node.FreezeNode(freezeDuration);
                    freezeMode = false;
                    freezeCooldownTimer = freezeCooldown; // start cooldown after use
                    Debug.Log("Freeze used on: " + node.name);
                }
            }
        }
    }

    public void ActivateFreeze()
    {
        if (freezeCooldownTimer > 0f)
        {
            Debug.Log($"Freeze on cooldown: {freezeCooldownTimer:F1}s remaining");
            return;
        }

        freezeMode = true;
        Debug.Log("Select target for Freeze");
    }
}