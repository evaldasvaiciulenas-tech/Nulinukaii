using UnityEngine;
using UnityEngine.UI;

public class ConfettiEffect : MonoBehaviour
{
    private Color[] colors = { Color.red, Color.yellow, Color.green, Color.cyan, Color.magenta };

    void OnEnable()
    {
        for (int i = 0; i < 50; i++)
        {
            CreateParticle();
        }
    }

    void CreateParticle()
    {
        GameObject obj = new GameObject("Confetti");
        obj.transform.SetParent(transform, false);

        Image img = obj.AddComponent<Image>();
        img.color = colors[Random.Range(0, colors.Length)];

        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(15, 15);
        rect.anchoredPosition = new Vector2(Random.Range(-500f, 500f), Random.Range(200f, 600f));

        obj.AddComponent<ConfettiParticle>();
    }
}