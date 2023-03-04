using UnityEngine;

[CreateAssetMenu(fileName = "WaveData", menuName = "MyScriptables/WaveData")]
public class WaveData : ScriptableObject
{
    public bool pistolEnemies;
    public float pistolEnemyChance;
    public bool rifleEnemies;
    public float rifleEnemyChance;
    public GameObject enemyPrefab;
    public int enemyCount;
    public float minSpawnTime;
    public float maxSpawnTime;
}

