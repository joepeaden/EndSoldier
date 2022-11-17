using UnityEngine;

// Author: Joseph Peaden

/// <summary>
/// Holds data for actors.
/// </summary>
[CreateAssetMenu(fileName = "ActorData", menuName = "MyScriptables/ActorData")]
public class ActorData : ScriptableObject
{
	public int hitPoints;
	public float recoilControl; 
	public float slowWalkMoveForce;
	public float fastWalkMoveForce;
	public float sprintMoveForce;
	public float rotationTorque;
	public int moveToCoverSpeed;
	public float coverSensorDistance;
}
