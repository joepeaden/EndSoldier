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
	public float aimMoveForce;
	public float normalMoveForce;
	public float sprintMoveForce;
	public float rotationTorque;
}
