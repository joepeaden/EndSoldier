 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// Author: Joseph Peaden

/// <summary>
/// Base class for enemy actors.
/// </summary>
public class Enemy : MonoBehaviour
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
		// required settings for 2D navmesh to work correctly
		//navAgent = GetComponent<NavMeshAgent>();
		//navAgent.updateRotation = false;
		//navAgent.updateUpAxis = false;

		// temporary
		target = GameManager.Instance.GetPlayerGO();

		actor.AddCoverListener(ActorHasPotentialCover);
		actor.OnDeath.AddListener(HandleEnemyDeath);
	}

    private void Update()
    {
		actor.Move(target.transform.position);
		actor.UpdateAim(target.transform.position);

        if (actor.GetEquippedWeaponAmmo() <= 0)
        {
            actor.AttemptReload();
        }
        else if (!pauseFiring)//(actor.state[Actor.State.BodyRotationFinished] && !pauseFiring)
        {
            int numToFire = Random.Range(1, 7);

			StartCoroutine(FireBurst(numToFire));
        }
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


		yield return new WaitForSeconds(.5f);

		pauseFiring = false;
	}
}
