using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeathText : MonoBehaviour
{
    [SerializeField] float lifeSpan = 4;
    [SerializeField] TMP_Text text;
    float curLifeSpan = 0;
    float lifeSpanSpeed = 1f;

    Vector2 centerPos;

    float bobAmount = 0.03f;

    // Start is called before the first frame update
    void Start()
    {
        if (!text) text = GetComponent<TMP_Text>();
        curLifeSpan = lifeSpan;
        centerPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        curLifeSpan -= lifeSpanSpeed * Time.deltaTime;
        text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Sqrt(curLifeSpan / lifeSpan));

        transform.position = new Vector2(centerPos.x, centerPos.y + Mathf.Sin(Time.time * 3) * bobAmount);

        if (curLifeSpan <= 0)
        {
            curLifeSpan = 0;
            Destroy(gameObject);
        }
    }
}
