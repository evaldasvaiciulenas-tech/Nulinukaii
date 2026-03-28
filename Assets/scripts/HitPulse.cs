using UnityEngine;

public class HitPulse : MonoBehaviour
{
    private float duration = 0.4f;
    private float elapsed = 0f;
    private float maxScale = 10f;
    private SpriteRenderer sr;

    public void Init(Color color)
    {
        sr = GetComponent<SpriteRenderer>();
        sr.color = color;
        gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / duration;

        // Scale up
        float scale = Mathf.Lerp(0.3f, maxScale, t);
        transform.localScale = Vector3.one * scale;

        // Fade out
        Color c = sr.color;
        c.a = Mathf.Lerp(0.8f, 0f, t);
        sr.color = c;

        if (elapsed >= duration)
            Destroy(gameObject);
    }
}