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
	public float crouchDogeChance;
	public AudioClip woundSound1;
	public AudioClip woundSound2;
	public AudioClip woundSound3;
	public AudioClip woundSound4;
	public AudioClip woundSound5;
	public AudioClip woundSound6;
	public AudioClip deathSound1;
	public AudioClip deathSound2;
	public AudioClip deathSound3;
}
