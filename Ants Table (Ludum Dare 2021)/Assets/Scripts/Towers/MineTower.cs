using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineTower : Tower
{
    [SerializeField] GameObject sugarCube;
    [SerializeField] AudioSource mineNoise;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] float mineFrequency = 1f;
    [SerializeField] ParticleSystem spawn;
    [SerializeField] int sugarValue = 50;
    [SerializeField] SpriteRenderer filling;
    float mineTimer = 0;
    float mineMax = 10f;

    void Start()
    {
        health = maxHealth;
        if (!mineNoise) mineNoise = GetComponent<AudioSource>();
        if (!sr) sr = GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        if (mineTimer < mineMax)
        {
            mineTimer += mineFrequency * Time.deltaTime;
            float blackLevel = mineTimer/mineMax;
            //if (blackLevel < 0.2f) blackLevel = 0.2f;
            //sr.color = new Color (blackLevel, blackLevel, blackLevel, 1);
            filling.size = new Vector2(filling.size.x, mineTimer / mineMax);
        }
        else
        {
            mineTimer = 0;
            Mine();
        }
    }

    void Mine()
    {
        Transform newSugar = Instantiate(sugarCube, transform.position, transform.rotation, transform).transform;
        newSugar.position = newSugar.position + new Vector3(Random.Range(0, 2) * 2 - 1, Random.Range(0, 2) * 2 - 1, 0).normalized;
        newSugar.GetComponent<SugarCube>().SetValue(sugarValue);
        spawn.Play();
        mineNoise.Play();
    }
}
