using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeLeavingTown : MonoBehaviour
{
    [SerializeField] float sleepFreq, sleepAmp, moveSpeed;
    float sleepySinewave;
    Vector2 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        sleepySinewave += Time.deltaTime;
        transform.position = new Vector3(transform.position.x-moveSpeed*Time.deltaTime, startPos.y + Mathf.Sin(sleepySinewave * sleepFreq) * sleepAmp / 2);

    }
}
