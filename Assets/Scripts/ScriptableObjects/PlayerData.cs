using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "MyScriptables/PlayerData")]
public class PlayerData : ActorData
{
	public float aimMoveForce;
	public float normalMoveForce;
	public float sprintMoveForce;
	public float rotationTorque;
}
