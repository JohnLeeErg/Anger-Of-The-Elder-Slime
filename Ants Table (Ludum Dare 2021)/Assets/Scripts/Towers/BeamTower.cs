using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamTower : Tower
{

    [SerializeField] float range = 3;
    [SerializeField] float lineWidth = 0.1f;
    [SerializeField] Color lineColor;
    [SerializeField] Material lineMaterial;
    [SerializeField] AudioSource beamNoise;
    bool tracking = false;
    Transform enemyTarget = null;
    LineRenderer line;
    float zDepth = -5;
    float volumeMax;
    float volumeSpeed = 2f;

    float yOffset = 0.63f;
    float xOffset = 0.11f;


    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        if (!beamNoise) beamNoise = GetComponent<AudioSource>();
        volumeMax = beamNoise.volume;
        beamNoise.volume = 0;
        LineSetup();
    }

    void LineSetup()
    {
        gameObject.AddComponent<LineRenderer>();
        line = GetComponent<LineRenderer>();
        line.startWidth = lineWidth;
        line.startColor = lineColor;
        line.endColor = lineColor;
        line.endWidth = 0.05f;
        ResetLine(transform, transform);
        line.material = lineMaterial;
        line.sortingLayerName = "Foreground";
    }

    // Update is called once per frame
    void Update()
    {
        Tracking();
        BeamNoise();
    }

    void BeamNoise()
    {
        if(beamNoise.isPlaying)
        {
            if (beamNoise.volume < volumeMax)
            {
                beamNoise.volume += volumeSpeed*Time.deltaTime;
            }
            else
            {
                beamNoise.volume = volumeMax;
            }
        }
        else
        {
            beamNoise.volume = 0;

            /*
            if (beamNoise.volume > 0)
            {
                beamNoise.volume -= volumeSpeed * Time.deltaTime;
            }
            else
            {
                beamNoise.volume = 0;
            }
            */
        }
    }

    void Tracking()
    {
        if (tracking) //if the tower has a target
        {
            if(enemyTarget != null)
            {
                if (Vector2.Distance(transform.position, enemyTarget.position) > range)
                {
                    EndTracking();
                }
                else
                {
                    SetLine(transform, enemyTarget);
                    enemyTarget.GetComponent<Slime>().Damage(damage*Time.deltaTime);

                    //print("Do " + damage * Time.deltaTime + " damage to " + enemyTarget.name);
                }
            }
            else
            {
                EndTracking();
            }

        }
        else //if the tower doesn't have a target
        {

            //get all the colliders in range
            Collider2D[] enemyColliders = Physics2D.OverlapCircleAll(transform.position, range);

            //find the closest one that is a Slime
            foreach (Collider2D enemyCollider in enemyColliders)
            {

                if (enemyCollider.tag == "Slime")
                {
                    if (enemyTarget == null)
                    {
                        StartTracking(enemyCollider.transform);
                    }
                    else
                    {
                        if (Vector2.Distance(transform.position, enemyTarget.position) > Vector2.Distance(transform.position, enemyCollider.transform.position))
                        {
                            StartTracking(enemyCollider.transform);
                        }
                    }
                }
            }
        }
    }

    void SetLine(Transform t0, Transform t1)
    {
        line.SetPosition(0, new Vector3(t0.position.x + xOffset, t0.position.y + yOffset, zDepth));
        line.SetPosition(1, new Vector3(t1.position.x, t1.position.y, zDepth));
    }

    void ResetLine(Transform t0, Transform t1)
    {
        line.SetPosition(0, new Vector3(t0.position.x + xOffset, t0.position.y + yOffset, zDepth));
        line.SetPosition(1, new Vector3(t1.position.x + xOffset, t1.position.y + yOffset, zDepth));
    }

    void StartTracking(Transform target)
    {
        beamNoise.Play();
        enemyTarget = target;
        tracking = true;
    }

    void EndTracking()
    {
        beamNoise.Stop();
        tracking = false;
        enemyTarget = null;
        ResetLine(transform, transform);
    }
}
