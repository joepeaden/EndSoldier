using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "MyScriptables/PlayerData")]
public class PlayerData : ScriptableObject
{
	[SerializeField]
	public float aimMoveSpeed;
	[SerializeField]
	public float normalMoveSpeed;
	[SerializeField]
	public float sprintMoveSpeed;
}
