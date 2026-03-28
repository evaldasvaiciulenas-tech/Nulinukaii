using System.Collections.Generic;
using UnityEngine;

public class AIBot : MonoBehaviour
{
  
    public float aggression = 0.7f; // Agresyvumas(0 - nuolat ginasi,
                                    // 1 - nuolat puola

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

        List<NodePastatas> aiNodes = new List<NodePastatas>();
        List<NodePastatas> playerNodes = new List<NodePastatas>();
        List<NodePastatas> neutralNodes = new List<NodePastatas>();

        foreach (var node in nodes)
        {
            if (node.owner == NodePastatas.OwnerType.AI)
                aiNodes.Add(node);
            else if (node.owner == NodePastatas.OwnerType.Player)
                playerNodes.Add(node);
            else
                neutralNodes.Add(node);
        }

        if (aiNodes.Count == 0) return;

        // Sprendimas gintis ar atakuoti
        NodePastatas threatened = GetWeakestNode(aiNodes);
        bool shouldDefend = threatened != null && threatened.studentCount < 15;

        // Truputis RNG + Gynyba
        if (shouldDefend && Random.value > aggression)
        {
            NodePastatas helper = GetStrongestNode(aiNodes, threatened);
            if (helper != null)
            {
                helper.SendStudents(threatened);
                return;
            }
        }

        // Ataka
        NodePastatas from = GetStrongestNode(aiNodes);

        if (from == null || from.studentCount < 10) return;

        NodePastatas target = null;

        if (neutralNodes.Count > 0 && Random.value > 0.2f)
            target = GetWeakestNode(neutralNodes);
        else if (neutralNodes.Count > 0)
            target = neutralNodes[Random.Range(0, neutralNodes.Count)];
        else if (playerNodes.Count > 0)
            target = GetWeakestNode(playerNodes);

        if (target != null)
            from.SendStudents(target);
    }
    NodePastatas GetWeakestNode(List<NodePastatas> list)
    {
        List<NodePastatas> weakestList = new List<NodePastatas>();

        int min = int.MaxValue;

        foreach (var node in list)
        {
            if (node.studentCount < min)
            {
                min = node.studentCount;
                weakestList.Clear();
                weakestList.Add(node);
            }
            else if (node.studentCount == min)
            {
                weakestList.Add(node);
            }
        }

        if (weakestList.Count == 0)
            return null;

        return weakestList[Random.Range(0, weakestList.Count)];
    }

    NodePastatas GetStrongestNode(List<NodePastatas> list, NodePastatas exclude = null)
    {
        NodePastatas strongest = null;
        int max = -1;

        foreach (var node in list)
        {
            if (node == exclude) continue;

            if (node.studentCount > max)
            {
                max = node.studentCount;
                strongest = node;
            }
        }

        return strongest;
    }
}