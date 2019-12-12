using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemy;

	public struct Wave {
		public int spawnCount;
		// eventually have dictionary of enemies to spawn w/ spawn counts for each?
		public GameObject enemy;
		// Dictionary<int, GameObject> enemyTypes;
		public float multiplier;

		public Wave(int spawnCount, GameObject enemy, float multiplier)
		{
			this.spawnCount = spawnCount;
			this.enemy = enemy;
			this.multiplier = multiplier;	
		}
	};

	public float spawnDelay;
	public float spawnTimer;
	public static float survivingEnemies = 0;
	public static float totalEnemiesSpawned = 0; 
	public float waveBreakSpawnDelay;
	Wave[] waves;
	static int currentWave;

	void Start()
	{
		spawnTimer = spawnDelay;
		waveBreakSpawnDelay = 10f;
		survivingEnemies = 0;
		InitializeWaves();
	}

	void InitializeWaves()
	{
		waves = new Wave[5];
		waves[0] = new Wave (5, enemy, 1.0f);
		waves[1] = new Wave(15, enemy, 1.5f);
		waves[2] = new Wave(15, enemy, 2f);
		waves[3] = new Wave(30, enemy, 2.5f);
		waves[4] = new Wave(100, enemy, 3f);
		currentWave = 0;
	}

	void Update()
	{
		spawnTimer -= Time.deltaTime;
	
		// if have spawned all enemies in current wave
		if(totalEnemiesSpawned >= waves[currentWave].spawnCount)
		{
			// if player has killed all enemies in wave, next. Otherwise, wait 
			if(survivingEnemies <= 0)
			{
				spawnTimer = waveBreakSpawnDelay;
				NextWave();
			}
		}
		else if(spawnTimer <= 0)
		{
			// ugly place for this to be... clean up later.
			if(totalEnemiesSpawned == 0)
			{
				// if wave just starting, update wave text
				UIManager.instance.UpdateWaveUI(currentWave+1);
			}

			SpawnEnemy();
			spawnTimer = spawnDelay;
		}
	}

	private void NextWave()
	{
		// for now, only 5 waves
		if (currentWave == 4)
		{
			FlowManager.instance.GameOver();
		}

		currentWave++;
		totalEnemiesSpawned = 0;
	}

	private void SpawnEnemy()
	{
		Instantiate(waves[currentWave].enemy, transform.position, Quaternion.identity);
		survivingEnemies++;
		totalEnemiesSpawned++;
	}
}
