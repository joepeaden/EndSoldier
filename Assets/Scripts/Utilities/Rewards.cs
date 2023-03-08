using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rewards : MonoBehaviour
{
    public GameObject lootPrefab;
    public List<Transform> spawnPoints = new List<Transform>();

    private void Start()
    {
        GameplayUI.OnRewardsPicked.AddListener(HandleRewardPicked);
    }

    private void OnDestroy()
    {
        GameplayUI.OnRewardsPicked.RemoveListener(HandleRewardPicked);
    }
    
    private void HandleRewardPicked(string rewardKey)
    {
        int spawnPointIndex = Random.Range(0, spawnPoints.Count);
        Loot loot = Instantiate(lootPrefab, spawnPoints[spawnPointIndex].position, Quaternion.identity).GetComponent<Loot>();
        if (loot)
        {
            loot.lootType = rewardKey;
        }
        else
        {
            Debug.LogError("No loot script on loot object");
        }
    }
}
