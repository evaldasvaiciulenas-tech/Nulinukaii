using UnityEngine;

public class AIBot : MonoBehaviour
{
    public float actionInterval = 2f;
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= actionInterval)
        {
            timer = 0f;
            DoAction();
        }
    }

    void DoAction()
    {
        NodePastatas[] nodes = FindObjectsOfType<NodePastatas>();

        // surandam AI fakultetus
        System.Collections.Generic.List<NodePastatas> aiNodes = new System.Collections.Generic.List<NodePastatas>();

        foreach (var node in nodes)
        {
            if (node.owner == NodePastatas.OwnerType.AI && node.studentCount > 10)
            {
                aiNodes.Add(node);
            }
        }

        if (aiNodes.Count == 0) return;

        // random AI node
        NodePastatas from = aiNodes[Random.Range(0, aiNodes.Count)];

        // random target (ne AI)
        System.Collections.Generic.List<NodePastatas> targets = new System.Collections.Generic.List<NodePastatas>();

        foreach (var node in nodes)
        {
            if (node != from && node.owner != NodePastatas.OwnerType.AI)
            {
                targets.Add(node);
            }
        }

        if (targets.Count == 0) return;

        NodePastatas target = targets[Random.Range(0, targets.Count)];

        from.SendStudents(target);
    }
}