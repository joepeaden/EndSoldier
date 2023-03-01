 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
	// scriptable object time...
	public float shootPauseTimeMax;
	public float shootPauseTimeMin;
	public float maxBurstFrames;
	public float minBurstFrames;
	public float maxTimeToFindPlayer; 
	public float minTimeToFindPlayer;

	public bool activateOnStart;

	private Actor actor;
	public GameObject target;
	private bool pauseFiring;

    private void Awake()
    {
		actor = GetComponent<Actor>();
		actor.team = Actor.ActorTeam.Enemy;
    }

    private void Start()
	{
		actor.AddCoverListener(ActorHasPotentialCover);
		actor.OnDeath.AddListener(HandleEnemyDeath);

		if (activateOnStart)
        {
			Activate();
        }
	}

    private void Update()
    {
		if (actor.IsAlive && target != null)
		{
			if (TargetInRangeAndLOS())
			{
				actor.StopMoving();

				actor.UpdateActorRotation(target.transform.position);

				if (actor.GetEquippedWeaponAmmo() <= 0)
				{
					actor.AttemptReload();
				}
				else if (!pauseFiring)
				{
					int numToFire = (int)Random.Range(minBurstFrames, maxBurstFrames);

					StartCoroutine(FireBurst(numToFire));
				}
			}
			else
			{
				actor.Move(target.transform.position);
			}
		}
	}

	/// <summary>
	/// Check if the actor's current target is in range and in LOS
	/// </summary>
	/// <returns></returns>
	private bool TargetInRangeAndLOS()
    {
		// how to check target type? Maybe could also use for walking to sounds... in which case range would be 0, the exact position to move to
		// basically make this more generic

		InventoryWeapon weapon = actor.GetEquippedWeapon();

		// cast overlap sphere with radius = range to see if target is possibly in range
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, weapon.data.range, LayerMask.GetMask("Actors"), QueryTriggerInteraction.Collide);
		foreach (Collider c in hitColliders.Where(col => col.GetComponent<HitBox>() != null))
        {
			HitBox h = c.GetComponent<HitBox>();
			// if it's the target, then check if have LOS of target
			if (h.GetActor().gameObject == target)
            {
				Ray r = new Ray(transform.position, (target.transform.position - transform.position));
				RaycastHit[] hits = Physics.RaycastAll(r, weapon.data.range);

				RaycastHit[] targetHits = hits.Where(hit => hit.collider.GetComponent<HitBox>() != null && hit.collider.GetComponent<HitBox>().GetActor().gameObject == target).ToArray();
				RaycastHit[] blockHits = hits.Where(hit => hit.collider.gameObject.layer == (int) IgnoreLayerCollisions.CollisionLayers.HouseAndFurniture).ToArray();
				
				// should only be one or zero targetHits. Check if any blocking hit is closer than the target, if so, can't shoot 
				foreach (RaycastHit targetHit in targetHits)
                {
					foreach (RaycastHit blockHit in blockHits)
					{
						if (blockHit.distance < targetHit.distance)
						{
							return false;
						}
					}
				}

				return targetHits.Count() > 0;
            }
        }

		return false;
	}

	public void Activate()
    {
		actor.SetVisibility(true);

		if (actor.IsAlive)
		{
			pauseFiring = false;
			StartCoroutine(LookForTarget());
		}
	}

	public void DeActivate()
	{
		actor.SetVisibility(false);

		if (actor.IsAlive)
		{
			pauseFiring = true;
			target = null;
			StopAllCoroutines();

			// if remove this move order, the actor goes to last player position. Might want it to be like that down the line. Just something to consider.
			//actor.Move(transform.position);
		}
	}

	private IEnumerator LookForTarget()
    {
		// for now. Just pause for a moment and then find player.
		// eventually, can use a sphere collider for awareness trigger then raycast to see if target is not visible (hits wall instead)
		yield return new WaitForSeconds((int) Random.Range(minTimeToFindPlayer, maxTimeToFindPlayer));
		target = GameManager.Instance.GetPlayerGO();
		actor.target = target.GetComponent<Actor>().GetShootAtMeTransform();
		yield return null;
	}

	/// <summary>
	/// For now, just rotates the actor 90 degrees to look ded.
	/// </summary>
	private void HandleEnemyDeath()
    {
		StopAllCoroutines();

		// for now just make him look ded.
		//Quaternion q = new Quaternion();
		//q.eulerAngles = new Vector3(0, 0, 90);
		//transform.rotation = q;

		// remove collider and set rigidbody to no grav so no collisions
		//GetComponent<Rigidbody>().useGravity = false;
		//GetComponent<Collider>().enabled = false;
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

		while (numToFire > 0 && currentWeaponAmmo > 0)
        {
			// if it's the first shot, make sure to pass triggerpull param correctly.
            actor.AttemptAttack(true);
			currentWeaponAmmo = actor.GetEquippedWeaponAmmo();

			numToFire--;

			InventoryWeapon weapon = actor.GetEquippedWeapon();
			if (!weapon.data.isAutomatic)
			{
				yield return new WaitForSeconds(Random.Range(actor.data.minSemiAutoFireRate, actor.data.maxSemiAutoFireRate));
			}
            else
            {
				yield return null;
			}
		}

		yield return new WaitForSeconds(Random.Range(shootPauseTimeMin, shootPauseTimeMax));

		pauseFiring = false;
	}
}
