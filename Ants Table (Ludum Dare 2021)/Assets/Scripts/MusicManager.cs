using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] AudioSource mainSong, angerSong, windSFX;
    [SerializeField] float startVolume = 0.1f;
    [SerializeField] float fadeInSpeed = 0.1f;
    [SerializeField] float maxVolume = 0.7f;
    [SerializeField] float windDelay = 5;
    float curWindTimer = 0;
    bool waitingForWind = false;

    // Start is called before the first frame update
    void Start()
    {
        maxVolume = mainSong.volume;
        mainSong.volume = startVolume;
        mainSong.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainSong.volume < maxVolume && !waitingForWind)
        {
            mainSong.volume += fadeInSpeed * Time.deltaTime;
        }
        else if(waitingForWind)
        {
            curWindTimer += Time.deltaTime;
            if(curWindTimer > windDelay)
            {
                windSFX.Play();
                waitingForWind = false;
            }
        }
    }
    public void RageHappens()
    {
        mainSong.volume = 0;
        angerSong.Play();
        waitingForWind = true;
        curWindTimer = 0;
    }
}
