using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public static UnityEvent OnStopSpawning = new UnityEvent(); 

    public static WaveData data;
    public static int waveEnemiesSpawned;
    public static bool shouldSpawn;
    public Transform enemiesParent;

    public void Start()
    {
        StartCoroutine("BeginSpawning");
    }

    private IEnumerator BeginSpawning()
    {
        while (true)
        {
            while (shouldSpawn)
            {
                if (waveEnemiesSpawned >= data.enemyCount)
                {
                    shouldSpawn = false;
                    OnStopSpawning.Invoke();
                    break;
                }

                float waitTime = Random.Range(data.minSpawnTime, data.maxSpawnTime);
                yield return new WaitForSeconds(waitTime);

                GameObject enemyGO = Instantiate(data.enemyPrefab, transform.position, Quaternion.identity, enemiesParent);
                Enemy enemyScript = enemyGO.GetComponent<Enemy>();
                if (enemyScript)
                {
                    // should decide what kind of gear they have based on data
                    ;
                }
                else
                {
                    Debug.LogWarning("No enemy script found on enemy " + enemyGO.name);
                }

                waveEnemiesSpawned++;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public static void NextWave(WaveData newData)
    {
        Debug.Log("Starting next wave!");
        data = newData;
        waveEnemiesSpawned = 0;
        shouldSpawn = true;
    }
}
