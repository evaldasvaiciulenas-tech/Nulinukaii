using UnityEngine;

public class Student : MonoBehaviour
{
    public float speed = 3f;

    private NodePastatas target;
    private NodePastatas source;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }
    public void SetTarget(NodePastatas t, NodePastatas s)
    {
        target = t;
        source = s;

        if (sr == null)
            sr = GetComponent<SpriteRenderer>();

        if (source.owner == NodePastatas.OwnerType.Player)
            sr.color = source.playerColor;
        else if (source.owner == NodePastatas.OwnerType.AI)
            sr.color = source.aiColor;
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