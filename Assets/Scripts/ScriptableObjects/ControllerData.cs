using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ControllerData", menuName = "MyScriptables/ControllerData")]
public class ControllerData : ScriptableObject
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

	public float navAgentSpeed;
}
