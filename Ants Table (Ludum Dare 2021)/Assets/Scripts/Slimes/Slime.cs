using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Healthers
{
    public float speed = .1f;
    public bool dropSugar = true;
    [SerializeField] int sugarValue = 5;

    void Start()
    {
        health = maxHealth;
    }

    public override void Death()
    {
        Destroy(gameObject);
    }

    public override void DeathHelper()
    {
        if (dropSugar)
        {
            GameObject.FindGameObjectWithTag("Builder")?.GetComponent<Building>().SlimeDeath(sugarValue, transform.position, transform.rotation);
            dropSugar = false;
        }
    }

}
