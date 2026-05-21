using UnityEngine;
using UnityEngine.UI;

public class LoseConfettiEffect : MonoBehaviour
{
    private Color[] colors = { 
        new Color(0.8f, 0f, 0f), 
        new Color(0.6f, 0f, 0f), 
        new Color(1f, 0.2f, 0.2f), 
        new Color(0.4f, 0f, 0f) 
    };

    void OnEnable()
    {
        for (int i = 0; i < 50; i++)
        {
            CreateParticle();
        }
    }

    void CreateParticle()
    {
        GameObject obj = new GameObject("LoseParticle");
        obj.transform.SetParent(transform, false);

        Image img = obj.AddComponent<Image>();
        img.color = colors[Random.Range(0, colors.Length)];

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(15, 15);
        rect.anchoredPosition = new Vector2(Random.Range(-500f, 500f), Random.Range(200f, 600f));

        obj.AddComponent<ConfettiParticle>();
    }
}