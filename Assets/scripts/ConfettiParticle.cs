using UnityEngine;

public class ConfettiParticle : MonoBehaviour
{
    float speed;
    float rotSpeed;

    void Start()
    {
        speed = Random.Range(200f, 500f);
        rotSpeed = Random.Range(-180f, 180f);
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.unscaledDeltaTime);
        transform.Rotate(0, 0, rotSpeed * Time.unscaledDeltaTime);

        if (GetComponent<RectTransform>().anchoredPosition.y < -700f)
            Destroy(gameObject);
    }
}