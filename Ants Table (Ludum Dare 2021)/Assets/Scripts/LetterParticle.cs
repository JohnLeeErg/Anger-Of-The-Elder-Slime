using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LetterParticle : MonoBehaviour
{
    float lifetime, speed, wiggleAmp, wiggleFreq;
    Vector2 movementDirection;
    Vector2 wiggleDirection;
    float currentLife = 0;
    TextMesh textComp;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Initialize(string text, Color customColor, Vector2 customMovementDirection, float customLifetime, float customSpeed = 0, float customWiggleAmp = 0, float customWiggleFreq= 0)
    {
        textComp = GetComponent<TextMesh>();
        if (textComp) {
            textComp.text = text;
            textComp.color = customColor;
            lifetime = customLifetime;
            speed = customSpeed;
            wiggleAmp = customWiggleAmp;
            wiggleFreq = customWiggleFreq;
            movementDirection = customMovementDirection;
            wiggleDirection = Vector2.Perpendicular(customMovementDirection);
            currentLife = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 targetPosition = transform.position +(Vector3) movementDirection * speed * Time.deltaTime;
        targetPosition += (Vector3)wiggleDirection * Mathf.Sin(currentLife * wiggleFreq) * wiggleAmp / 2;
        transform.position = targetPosition;
        currentLife += Time.deltaTime;
        if (currentLife > lifetime)
        {
            Destroy(gameObject);
        }
    }
}
