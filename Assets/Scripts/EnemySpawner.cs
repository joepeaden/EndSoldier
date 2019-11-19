using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemy;

	public float spawnDelay;
	public float spawnTimer;
	public static float survivingEnemies = 0;
	public static float totalEnemiesSpawned = 0; 
	public float waveBreakSpawnDelay;
	public int waveSpawnLimit;

	void Start()
	{
		spawnTimer = spawnDelay;
		waveBreakSpawnDelay = 10f;
		waveSpawnLimit = 10;
		survivingEnemies = 0;
		totalEnemiesSpawned = 0;
	}

	void Update()
	{
		spawnTimer -= Time.deltaTime;
		if(totalEnemiesSpawned >= waveSpawnLimit)
		{
			spawnTimer = waveBreakSpawnDelay;
			waveSpawnLimit += waveSpawnLimit; 
		}
		else if(spawnTimer <= 0)
		{
			// new wave
			if(totalEnemiesSpawned >= waveSpawnLimit)
			{
				spawnTimer = spawnDelay;
			}
			else
			{
				SpawnEnemy();
				spawnTimer = spawnDelay;
			}
		}
	}

	private void SpawnEnemy()
	{
		Instantiate(enemy, transform.position, Quaternion.identity);
		survivingEnemies++;
		totalEnemiesSpawned++;
	}
}
