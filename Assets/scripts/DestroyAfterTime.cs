using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    public float lifetime = 1f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }
}