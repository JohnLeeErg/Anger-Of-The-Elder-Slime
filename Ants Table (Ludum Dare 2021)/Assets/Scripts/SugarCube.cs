using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarCube : MonoBehaviour
{
    [SerializeField] int value = 50;
    [SerializeField] float lifeSpan = 10;
    SpriteRenderer sr;
    float curLifeSpan = 0;
    float lifeSpanSpeed = 1f;
    float bobAmount = 0.05f;
    Vector2 centerPos;

    void Start()
    {
        if (!sr) sr = GetComponent<SpriteRenderer>();
        curLifeSpan = lifeSpan;

        centerPos = transform.position;
    }


    void Update()
    {
        curLifeSpan -= lifeSpanSpeed * Time.deltaTime;
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, Mathf.Sqrt(curLifeSpan / lifeSpan));

        transform.position = new Vector2(centerPos.x, centerPos.y + Mathf.Sin(Time.time*3) * bobAmount);

        if (curLifeSpan <= 0)
        {
            curLifeSpan = 0;
            Destroy(gameObject);
        }
    }
    void OnMouseDown()
    { 
        GameObject.FindGameObjectWithTag("Builder").GetComponent<Building>().PickupSugar(value);
        Destroy(gameObject);
    }

    public void SetValue(int amount)
    {
        value = amount;
    }
}
