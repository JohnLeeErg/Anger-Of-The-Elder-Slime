using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "ScriptableObjects/MonsterWave", order = 1)]
public class Wave : ScriptableObject
{
    public float timeBeforeWave = 2;
    public float timeAfterWave = 0;

    public float minSlimeSpawnStagger = .1f;
    public float maxSlimeSpawnStagger = .3f;

    public bool skipAfterTimeIfNoSlimes = true;

    public GameObject[] monsterWave;

}
