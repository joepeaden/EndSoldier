using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public static int waveEnemiesSpawned;
    public static bool shouldSpawn;
    public Transform enemiesParent;

    public static int waveNumber;

    #region Current Wave Variables
    /// <summary>
    /// how many enemies that should be spawned this wave
    /// </summary>
    private static int currentEnemyCountGoal;
    /// <summary>
    /// The current list of enemy prefabs to be chosen from (based on current wave)
    /// </summary>
    private static List<GameObject> spawnableEnemyPrefabs = new List<GameObject>();

    #endregion

    private static WaveData data;

    public void Start()
    {
        data = WaveManager.Instance.GetWaveData();

        GameManager.OnGameOver.AddListener(Reset);

        StartCoroutine("BeginSpawning");
    }

    /// <summary>
    /// Reset at game over
    /// </summary>
    private void Reset()
    {
        waveNumber = 0;
    }

    private IEnumerator BeginSpawning()
    {
        while (true)
        {
            while (shouldSpawn)
            {
                float waitTime = Random.Range(data.minSpawnTime, data.maxSpawnTime);
                yield return new WaitForSeconds(waitTime);

                int randomEnemyIndex = Random.Range(0, spawnableEnemyPrefabs.Count);

                // here because it's after the waitforseconds, so should be no accidental spawns after over the limit
                if (waveEnemiesSpawned >= currentEnemyCountGoal)
                {
                    shouldSpawn = false;
                    break;
                }
            
                Instantiate(spawnableEnemyPrefabs[randomEnemyIndex], transform.position, Quaternion.identity, enemiesParent);

                waveEnemiesSpawned++;
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public static void NextWave()
    {
        Debug.Log("Starting next wave!");
        waveNumber++;

        UpdateWaveVariables();

        waveEnemiesSpawned = 0;
        shouldSpawn = true;
    }

    private static void UpdateWaveVariables()
    {
        // Increase enemy count
        if (waveNumber == 1)
        {
            currentEnemyCountGoal = data.baseEnemyCount;
        }
        else
        {
            currentEnemyCountGoal = (int)(data.baseEnemyCount * data.wavePopulationMultiplier * (waveNumber - 1));
        }

        // Add whatever enemies are appropriate based on the wave count
        if (waveNumber >= data.enemyPistolIntroWave && !spawnableEnemyPrefabs.Contains(data.enemyPistolPrefab))
        {
            spawnableEnemyPrefabs.Add(data.enemyPistolPrefab);
        }
        if (waveNumber >= data.enemyRifleIntroWave && !spawnableEnemyPrefabs.Contains(data.enemyRiflePrefab))
        {
            spawnableEnemyPrefabs.Add(data.enemyRiflePrefab);
        }
    }
}
