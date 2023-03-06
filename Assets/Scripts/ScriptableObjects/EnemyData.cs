using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "MyScriptables/EnemyData")]
public class EnemyData : ScriptableObject
{
    public int minSemiAutoFireRate;
    public int maxSemiAutoFireRate;

    public int scoreValue;

	public float shootPauseTimeMax;
	public float shootPauseTimeMin;
	public float maxBurstFrames;
	public float minBurstFrames;
	public float maxTimeToFindPlayer;
	public float minTimeToFindPlayer;
}
