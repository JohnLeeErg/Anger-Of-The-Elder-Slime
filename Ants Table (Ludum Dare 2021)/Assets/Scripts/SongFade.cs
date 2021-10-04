using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongFade : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    [SerializeField] float startVolume = 0.1f;
    [SerializeField] float fadeInSpeed = 0.1f;
    [SerializeField] float maxVolume = 0.7f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.volume = startVolume;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(audioSource.volume < maxVolume)
        {
            audioSource.volume += fadeInSpeed * Time.deltaTime;
        }
    }
}
