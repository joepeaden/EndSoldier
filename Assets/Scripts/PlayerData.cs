using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "MyScriptables/PlayerData")]
public class PlayerData : ScriptableObject
{
	[SerializeField]
	public float aimMoveForce;
	[SerializeField]
	public float normalMoveForce;
	[SerializeField]
	public float sprintMoveForce;
	[SerializeField]
	public int hitPoints;
	public float recoilControl;
}
