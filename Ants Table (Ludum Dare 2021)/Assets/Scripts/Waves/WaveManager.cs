using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveManager : MonoBehaviour
{

    public Wave[] waves;
    public int spawnRightOffset = 1;
    public float NewWaveTextfadeOutSec=2;

    public TMP_Text waveText;
    public TMP_Text newWaveTextFlash;
    public TMP_Text newWaveNumbersFlash;
    public TMP_Text endGameText;


    public float FirstWaveDelaySeconds = 2f;

    private int currentWave = 0;

    SlimeAngerManager slimeAngerManager;

    [SerializeField] bool goToEnding = false;

    float endTimer = 5;
    bool timeToEnd = false;

    int maxRow = 0;
    int minRow = 0;

    int startingX = 0;

    // Start is called before the first frame update
    void Start()
    {
        slimeAngerManager = SlimeAngerManager.instance;
        updateWaveText();
        StartCoroutine(WaveGenerator());
        newWaveTextFlash.transform.parent.gameObject.SetActive(false);
        endGameText.transform.parent.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.S))
        //{
        //StopAllCoroutines();
        // StartCoroutine(WaveGenerator());
        //}

        if(timeToEnd)
        {
            if(endTimer > 0)
            {
                endTimer -= Time.deltaTime;
                //print("endTime: "+ endTimer);
                endGameText.text = "You Won!\nContinuing in: \n"+Mathf.Round(endTimer);
            }
            else
            {
                endGameText.transform.parent.gameObject.SetActive(false);
                sceneManager.instance.loadNextLevel();
            }

        }
    }

    void SpawnSlime(GameObject slime)
    {
        print("Spawning!");
        Vector3 spawnPosition = new Vector3(startingX, Random.Range(minRow, maxRow), 0);
        GameObject babySlime = Instantiate(slime, spawnPosition, Quaternion.identity, gameObject.transform);
        babySlime.GetComponent<SlimePathfinding>().MIN_ROW = minRow;
        babySlime.GetComponent<SlimePathfinding>().MAX_ROW = maxRow;
        babySlime.GetComponent<SlimePathfinding>().startingPos = spawnPosition;
    }


    IEnumerator WaveGenerator()
    {
        yield return new WaitForSeconds(FirstWaveDelaySeconds);

        maxRow = slimeAngerManager.TopLeftOfGrid.y;
        minRow = slimeAngerManager.BottomRightOfGrid.y;

        startingX = slimeAngerManager.BottomRightOfGrid.x + spawnRightOffset;
        print("startingX:" + startingX);

        for (int i = 0; i < waves.Length; i++)
        {
            currentWave = i + 1;

            Wave wave = waves[i];
            updateWaveText();
            StartCoroutine(TextFlash());
            GetComponent<AudioSource>().Play();

            yield return new WaitForSeconds(wave.timeBeforeWave);

            for (int j = 0; j < wave.monsterWave.Length; j++)
            {
                yield return new WaitForSeconds(Random.Range(wave.minSlimeSpawnStagger, wave.maxSlimeSpawnStagger));
                SpawnSlime(wave.monsterWave[j]);
            }
            for (float k = 0; k < wave.timeAfterWave; k += 0.1f)
            {
                yield return new WaitForSeconds(.1f);
                if (transform.childCount == 0 && wave.skipAfterTimeIfNoSlimes)
                {
                    print("Finishing wave early, no slimes left");
                    k = wave.timeAfterWave;
                }
            }
        }

        print("You have finished the level!");
        if (goToEnding)
        {
            timeToEnd = true;
            print("showing end text");
            endGameText.transform.parent.gameObject.SetActive(true);
        }
    }

    void updateWaveText()
    {
        waveText.text = "" + currentWave + "/" + waves.Length;
    }

    IEnumerator TextFlash()
    {
        newWaveNumbersFlash.text = currentWave + "/" + waves.Length;

        newWaveTextFlash.transform.parent.gameObject.SetActive(true);
        float step = NewWaveTextfadeOutSec / 100;
        for (float i = 0; i < 1; i += .01f)
        {
            float percent = (1 - i);
            newWaveTextFlash.alpha = percent;
            newWaveNumbersFlash.alpha = percent;
            Color color = newWaveTextFlash.GetComponentInParent<Image>().color;
            color.a = .3f * percent;
            newWaveTextFlash.GetComponentInParent<Image>().color = color;
            yield return new WaitForSeconds(step);
        }
        newWaveTextFlash.transform.parent.gameObject.SetActive(false);
    }
    
}
