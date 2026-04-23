using UnityEngine;

public class LevelStudentConfig : MonoBehaviour
{
    public Sprite playerSprite;
    public Sprite aiSprite;

    public Student studentPrefab; // drag your student prefab here

    void Awake()
    {
        studentPrefab.playerSprite = playerSprite;
        studentPrefab.aiSprite = aiSprite;
    }
}