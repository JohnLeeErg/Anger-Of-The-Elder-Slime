using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaTower : Tower
{
    [SerializeField] float range = 3;
    [SerializeField] AudioSource stompNoise;
    [SerializeField] float stompFrequency = 1f;
    [SerializeField] Transform hitEffect;
    SpriteRenderer hitEffectSR;
    ParticleSystem hitEffectParticles;
    float stompTimer = 0;
    float stompMax = 10f;
    Vector2 center;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        if (!stompNoise) stompNoise = GetComponent<AudioSource>();
        hitEffectSR = hitEffect.GetComponent<SpriteRenderer>();
        hitEffectParticles = GetComponent<ParticleSystem>();
        hitEffectSR.enabled = false;
        center = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if(stompTimer > stompMax*0.15f)
        {
            hitEffectSR.enabled = false;
        }
        else if (stompTimer > 0)
        {
            hitEffectSR.transform.localScale = new Vector3(stompTimer/(stompMax * 0.15f), stompTimer / (stompMax * 0.15f), stompTimer / (stompMax * 0.15f));
        }

        if (stompTimer < stompMax)
        {
            stompTimer += stompFrequency * Time.deltaTime;

            if (stompTimer > stompMax*0.90 && stompTimer < stompMax * 0.95)
            {
                transform.position = new Vector2 (center.x, center.y + (stompTimer - stompMax * 0.90f));
            }
            else if (stompTimer > stompMax * 0.95)
            {
                transform.position = new Vector2(center.x, center.y - (0.5f + (stompMax * 0.95f - stompTimer)));
            }
        }
        else
        {
            stompTimer = 0;
            Stomp();
        }
    }

    void Stomp()
    {
        transform.position = center;
        stompNoise.Play();
        hitEffectSR.enabled = true;
        hitEffectSR.transform.localScale = Vector3.zero;
        hitEffectParticles.Play();

        //get all the colliders in range
        Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(transform.position, range);

        //find the closest one that is a Slime
        foreach (Collider2D enemyCollider in enemyColliders)
        {

            if (enemyCollider.tag == "Slime")
            {
                enemyCollider.transform.GetComponent<Slime>().Damage(damage);
                //print("Do " + damage + " damage to " + enemyCollider.name);
            }
        }
    }
}
