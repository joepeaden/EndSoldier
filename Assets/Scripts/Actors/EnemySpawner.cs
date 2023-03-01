using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public SpawnerData data;

    public Transform enemiesParent;

    public void Start()
    {
        StartCoroutine("BeginSpawning");
    }

    private IEnumerator BeginSpawning()
    {
        while (true)
        {
            float waitTime = Random.Range(data.minSpawnTime, data.maxSpawnTime);
            yield return new WaitForSeconds(waitTime);

            Instantiate(data.spawnObject, transform.position, Quaternion.identity, enemiesParent);
        } 
    }
}
