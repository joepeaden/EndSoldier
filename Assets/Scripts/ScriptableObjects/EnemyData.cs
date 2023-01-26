using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "MyScriptables/EnemyData")]
public class EnemyData : ActorData
{
    public int minSemiAutoFireRate;
    public int maxSemiAutoFireRate;
}
