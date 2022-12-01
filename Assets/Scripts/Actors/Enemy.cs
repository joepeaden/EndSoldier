﻿ using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Author: Joseph Peaden

/// <summary>
/// Base class for enemy actors.
/// </summary>
/// <remarks>
/// It probably will be worth it to have an AIActor subclass and take some of the functionality from here.
/// Stuff like the NavMesh and "perception"
/// </remarks>
public class Enemy : MonoBehaviour, ISetActive
{
	private Actor actor;
	private GameObject target;
	private bool pauseFiring;

    private void Awake()
    {
		actor = GetComponent<Actor>();
    }

    private void Start()
	{
		actor.AddCoverListener(ActorHasPotentialCover);
		actor.OnDeath.AddListener(HandleEnemyDeath);
	}

    private void Update()
    {
		if (target != null)
		{
			// just disabling chasing for now
			//actor.Move(target.transform.position);

			actor.UpdateAim(target.transform.position);

			if (actor.GetEquippedWeaponAmmo() <= 0)
			{
				actor.AttemptReload();
			}
			else if (!pauseFiring)
			{
				int numToFire = Random.Range(1, 4);

				StartCoroutine(FireBurst(numToFire));
			}
		}
    }

	public void Activate()
    {
		if (actor.IsAlive)
		{
			pauseFiring = false;
			actor.SetVisibility(true);

			StartCoroutine(LookForTarget());
		}
	}

	public void DeActivate()
	{
		if (actor.IsAlive)
		{
			pauseFiring = true;
			target = null;
			StopAllCoroutines();

			actor.SetVisibility(false);

			// if remove this move order, the actor goes to last player position. Might want it to be like that down the line. Just something to consider.
			actor.Move(transform.position);
		}
	}

	private IEnumerator LookForTarget()
    {
		// for now. Just pause for a moment and then find player.
		// eventually, can use a sphere collider for awareness trigger then raycast to see if target is not visible (hits wall instead)
		yield return new WaitForSeconds(1f);
		target = GameManager.Instance.GetPlayerGO();
		yield return null;
	}

	/// <summary>
	/// For now, just rotates the actor 90 degrees to look ded.
	/// </summary>
	private void HandleEnemyDeath()
    {
		// for now just make him look ded.
		Quaternion q = new Quaternion();
		q.eulerAngles = new Vector3(0, 0, 90);
		transform.rotation = q;

		this.enabled = false;
	}

	private void ActorHasPotentialCover()
    {
		Debug.Log("Enemy sees potential cover");
    }

	private IEnumerator FireBurst(int numToFire)
    {
		pauseFiring = true;

		int initialWeaponAmmo = actor.GetEquippedWeaponAmmo();
		int currentWeaponAmmo = initialWeaponAmmo;

		while (initialWeaponAmmo - currentWeaponAmmo < numToFire && currentWeaponAmmo > 0)
        {
            actor.AttemptAttack();
			currentWeaponAmmo = actor.GetEquippedWeaponAmmo();

			yield return null;
        }


		yield return new WaitForSeconds(1f);

		pauseFiring = false;
	}
}
