using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	public GameObject enemy;

	public float spawnDelay;
	private float spawnTimer;

	void Start()
	{
		spawnTimer = spawnDelay;
	}

	void Update()
	{
		spawnTimer -= Time.deltaTime;
		if(spawnTimer <= 0)
		{
			SpawnEnemy();
			spawnTimer = spawnDelay;
		}
	}

	private void SpawnEnemy()
	{
		Instantiate(enemy, transform.position, Quaternion.identity);
	}
}
