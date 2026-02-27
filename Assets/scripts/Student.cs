using UnityEngine;

public class Student : MonoBehaviour
{
    public float speed = 3f;

    private NodePastatas target;
    private NodePastatas source;

    public void SetTarget(NodePastatas t, NodePastatas s)
    {
        target = t;
        source = s;
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