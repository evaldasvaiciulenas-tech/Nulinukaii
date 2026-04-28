using UnityEngine;

public class Student : MonoBehaviour
{
    public float speed = 3f;

    public Sprite playerSprite;
    public Sprite aiSprite;

    private NodePastatas target;
    private NodePastatas source;
    private SpriteRenderer sr;
    public NodePastatas GetSource() => source;

    public void ApplySpeedBoost(float multiplier, float duration)
    {
        StartCoroutine(SpeedBoostCoroutine(multiplier, duration));
    }

    private System.Collections.IEnumerator SpeedBoostCoroutine(float multiplier, float duration)
    {
        speed *= multiplier;
        yield return new WaitForSeconds(duration);
        speed /= multiplier;
    }

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetTarget(NodePastatas t, NodePastatas s)
    {
        target = t;
        source = s;

        if (sr == null) sr = GetComponent<SpriteRenderer>();

        if (source.owner == NodePastatas.OwnerType.Player && playerSprite != null)
            sr.sprite = playerSprite;
        else if (source.owner == NodePastatas.OwnerType.AI && aiSprite != null)
            sr.sprite = aiSprite;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        float travelTime = distance / 2f;
        speed = distance / travelTime;

        if (source.owner == NodePastatas.OwnerType.Player &&
            AbilityManager.Instance != null &&
            AbilityManager.Instance.IsSpeedActive)
        {
            speed *= AbilityManager.Instance.speedMultiplier;
        }
    }

    void Update()
    {
        if (target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.transform.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.transform.position) < 0.1f)
        {
            target.ReceiveStudent(source);
            source.StudentArrived();
            Destroy(gameObject);
        }
    }

}