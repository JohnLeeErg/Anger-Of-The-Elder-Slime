using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTower : Tower
{
    [SerializeField] GameObject bullet;
    [SerializeField] AudioSource shootNoise;
    [SerializeField] float shootFrequency = 1f;
    [SerializeField] float bulletSpeed;
    float shootTimer = 0;
    float shootMax = 10f;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        if (!shootNoise) shootNoise = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

        if (shootTimer < shootMax)
        {
            shootTimer += shootFrequency * Time.deltaTime;
        }
        else
        {
            shootTimer = 0;
            Shoot();
        }
    }

    void Shoot()
    {
        Rigidbody2D newBullet = Instantiate(bullet, transform.position, transform.rotation, transform).GetComponent<Rigidbody2D>();
        newBullet.GetComponent<Bullet>().SetDamage(damage);
        newBullet.AddForce(Vector2.right * bulletSpeed);
        shootNoise.Play();
    }
}
