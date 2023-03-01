using UnityEngine;

[CreateAssetMenu(fileName = "SpawnerData", menuName = "MyScriptables/SpawnerData")]
public class SpawnerData : ScriptableObject
{
    public GameObject spawnObject;
    public float minSpawnTime;
    public float maxSpawnTime;
}

