using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeMe : MonoBehaviour
{
    Vector3 originalPosition;
    bool shaking = false;
    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;
    }

    // Update is called once per frame
    public void ShakeObjectForDuration(float strength,float duration)
    {
        transform.localPosition = originalPosition;
        StopAllCoroutines();
        shaking = true;
        StartCoroutine(ShakeDurationCoroutine(strength, duration));
    }
    IEnumerator ShakeDurationCoroutine(float strength, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            transform.localPosition = originalPosition+ new Vector3(Random.Range(-strength, strength), Random.Range(-strength, strength));
            t += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
        shaking = false;
    }

    public void ShakeObjectIndefinitely(float strength)
    {
        transform.localPosition = originalPosition;
        StopAllCoroutines();
        shaking = true;
        StartCoroutine(ShakeUntilStoppedCoroutine(strength));
    }
    IEnumerator ShakeUntilStoppedCoroutine(float strength)
    {
        while (shaking)
        {
            transform.localPosition = originalPosition + new Vector3(Random.Range(-strength, strength), Random.Range(-strength, strength));
            
            yield return null;
        }
        transform.localPosition = originalPosition;
    }

    public void StopShaking()
    {
        shaking = false;
    }
}
