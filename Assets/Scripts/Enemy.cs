 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Author: Joseph Peaden

/// <summary>
/// Base class for enemy actors.
/// </summary>
public class Enemy : Actor
{
	private NavMeshAgent navAgent;
	private GameObject target;
	private bool pauseFiring;

	private void Start()
	{
		// required settings for 2D navmesh to work correctly
		//navAgent = GetComponent<NavMeshAgent>();
		//navAgent.updateRotation = false;
		//navAgent.updateUpAxis = false;

		// temporary
		target = GameManager.Instance.GetPlayerGO();
	}

    private void Update()
    {
		Vector3 direction = (target.transform.position - transform.position).normalized;

		UpdateAim(direction);

		if (!weapon.HasAmmo())
		{
			weapon.StartReload();
		} 
		else if (upperBodyFinishedRotating && !pauseFiring)
        {
			int numToFire = Random.Range(1, 7);

			StartCoroutine(FireBurst(numToFire));
        }

		// chase player
		//navAgent.destination = FindObjectOfType<Player>().transform.position;

		//// update aim rotation
		//Quaternion q = new Quaternion();
		//Vector3 v3 = navAgent.path.corners[1];
		//torsoT.right = navAgent.path.corners[1] - transform.position;
    }

	private IEnumerator FireBurst(int numToFire)
    {
		pauseFiring = true;

		int initialWeaponAmmo = weapon.GetAmmo();
		int currentWeaponAmmo = initialWeaponAmmo;

		while (initialWeaponAmmo - currentWeaponAmmo < numToFire && currentWeaponAmmo > 0)
        {
            weapon.InitiateAttack(data.recoilControl);
			currentWeaponAmmo = weapon.GetAmmo();

			yield return null;
        }


		yield return new WaitForSeconds(.5f);

		pauseFiring = false;
	}
}
