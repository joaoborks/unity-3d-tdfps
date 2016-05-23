using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class WaveController : MonoBehaviour
{
    public GameObject[] enemyPrefabs;
    public Transform[] path1,
        path2,
        path3,
        path4;
    public Transform enemyOrigin;
    public Text wave;

    public static int curEnemies;

    List<Transform[]> paths = new List<Transform[]>(4);
    PlayerBuilder builder;
    PlayerCombat pc;
    PauseMenu menu;
    float spawnInterval = .5f;
    int maxWaves = 3,
        curWave = 1,
        maxEnemiesWaveNum = 5,
        curSpawnEnemies,
        creditsWave = 1500,
        basesWave = 3;

    void Awake()
    {
        builder = FindObjectOfType<PlayerBuilder>();
        pc = builder.GetComponent<PlayerCombat>();
        menu = FindObjectOfType<PauseMenu>();
        wave.text = "Wave " + curWave + "/" + maxWaves;
        curEnemies = 0;
        paths.Add(path1);
        paths.Add(path2);
        paths.Add(path3);
        paths.Add(path4);
    }

    public void BeginWave()
    {
        builder.enabled = false;
        StartCoroutine(ControlWave());
    }

    public void EndWave()
    {
        if (curWave == maxWaves)
            Victory();
        else
        {
            builder.enabled = true;
            builder.UpdateResources(creditsWave * curWave, basesWave * curWave);
        }
        curWave++;
        wave.text = "Wave " + curWave + "/" + maxWaves;
        pc.ChangeAmmo(150);
        pc.ChangeHealth(100);
    }

    public void Victory()
    {
        menu.SetActive(1);
    }

    IEnumerator ControlWave()
    {
        curSpawnEnemies = 0;
        while (curSpawnEnemies < maxEnemiesWaveNum * curWave)
        {
            IPathWalkable mob = ((GameObject)Instantiate(enemyPrefabs[Random.Range(0, enemyPrefabs.Length)], enemyOrigin.position, enemyOrigin.rotation)).GetComponentInChildren<IPathWalkable>();
            mob.SetPath(paths[Random.Range(0, paths.Count)]);
            curSpawnEnemies++;
            curEnemies++;
            yield return new WaitForSeconds(spawnInterval);
        }
        while (curEnemies > 0)
            yield return new WaitForEndOfFrame();
        EndWave();
    }
}