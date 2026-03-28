using UnityEngine;

public class MaxPulse : MonoBehaviour
{
    private SpriteRenderer sr;
    private float speed = 2f;
    private float minScale = 2.5f;
    private float maxScale = 5f;
    private float minAlpha = 0f;
    private float maxAlpha = 0.5f;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {

        float t = (Mathf.Sin(Time.time * speed) + 1f) / 2f;

        float scale = Mathf.Lerp(minScale, maxScale, t);
        transform.localScale = Vector3.one * scale;

        Color c = sr.color;
        c.a = Mathf.Lerp(minAlpha, maxAlpha, t);
        sr.color = c;
    }
}