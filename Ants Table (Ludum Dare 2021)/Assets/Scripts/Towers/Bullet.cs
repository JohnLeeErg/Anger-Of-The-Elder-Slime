using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage = 1;
    float bobAmount = 0.03f;
    Vector2 centerPos;

    void Start()
    {
        centerPos = transform.position;
    }

    void Update()
    {
        transform.position = new Vector2(transform.position.x, centerPos.y + Mathf.Sin(Time.time * 5) * bobAmount);
    }
    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "Slime")
        {
            //print("Do " + damage + " damage to " + collision.transform.name);
            collision.transform.GetComponent<Slime>().Damage(damage);

            Destroy(gameObject);
        }
        else if (collision.transform.tag == "Solid")
        {
            Destroy(gameObject);
        }
    }
}
