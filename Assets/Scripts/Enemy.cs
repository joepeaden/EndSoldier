 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	[SerializeField] Transform torsoT;
	
	private NavMeshAgent navAgent;

	private void Start()
	{
		// required settings for 2D navmesh to work correctly
		navAgent = GetComponent<NavMeshAgent>();
		navAgent.updateRotation = false;
		navAgent.updateUpAxis = false;
	}

    private void Update()
    {
		// chase player
		navAgent.destination = FindObjectOfType<Player>().transform.position;

		// update aim rotation
		Quaternion q = new Quaternion();
		Vector3 v3 = navAgent.path.corners[1];
		torsoT.right = navAgent.path.corners[1] - transform.position;
    }
}
