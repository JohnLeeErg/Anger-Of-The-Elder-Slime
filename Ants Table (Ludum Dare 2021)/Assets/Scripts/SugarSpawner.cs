using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SugarSpawner : MonoBehaviour
{
    [SerializeField] GameObject pinkCube, whiteCube;
    [SerializeField] Transform botLeft, topRight;
    [SerializeField] ParticleSystem particles;
    [SerializeField] float cubeFrequency = 1;
    [SerializeField] float pinkChanceDenom = 8;
    [SerializeField] AudioSource spawn;
    [SerializeField] Color pink;
    float cubeTimer = 0;
    float cubeMax = 10f;

    void Update()
    {
        if (cubeTimer < cubeMax)
        {
            cubeTimer += cubeFrequency * Time.deltaTime;
        }
        else
        {
            cubeTimer = 0;
            SpawnCube();
        }
    }

    void SpawnCube()
    {
        Transform newSugar;

        particles.Stop();

        Vector3 pos = new Vector3(Random.Range(botLeft.position.x, topRight.position.x), Random.Range(botLeft.position.y, topRight.position.y), 0);
        particles.transform.position = pos;

        if (Random.Range(0f, 1f)* pinkChanceDenom < 1)
        {
            newSugar = Instantiate(pinkCube, transform.position, transform.rotation, transform).transform;
            particles.startColor = pink;
        }
        else
        {
            newSugar = Instantiate(whiteCube, transform.position, transform.rotation, transform).transform;
            particles.startColor = Color.white;
        }

        particles.Play();

        newSugar.position = pos;
        spawn.Play();
    }
}
