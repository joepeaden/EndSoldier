using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoota : Actor
{
	public GameObject player;
	public Quaternion aimAngle;
	public GameObject missileRange;

	public enum State { SHOOTING, MOVING };
	public State state = State.MOVING; 

	void Start()
	{
		upperBody = this.gameObject;
		fireRateTimer = fireRate;
	}

	void Update()
	{
		fireRateTimer -= Time.deltaTime;
		switch(state)
		{
			case State.SHOOTING:
				if(fireRateTimer <= 0)
				{
					ShootAtPlayer();
					fireRateTimer = fireRate;
				}
				break;
			case State.MOVING:
				MoveTowardsPlayer();
				break;
		}
	}

	public void MoveTowardsPlayer()
	{
		Vector3 playerVector = GetPlayerVector();
		transform.Translate(-moveSpeed * playerVector * Time.deltaTime);
	}

	/// SHOOTING ///

	public void TargetInRange()
	{
		state = State.SHOOTING;
	}

	public void TargetOutRange()
	{
		state = State.MOVING;
	}

	public void ShootAtPlayer()
	{
		UpdateAim(Vector2.zero);
		FireProjectile();
	}

	// gets unit vector from this object to player
	public Vector3 GetPlayerVector()
	{
		Vector3 playerVector = transform.position - player.transform.position;
		playerVector = Vector3.Normalize(playerVector);
		return playerVector;
	}

	// upperbody angle is used for aiming missile fire
	protected override void UpdateAim(Vector2 targetPos)
	{
		// for some reason, playervector needs to be negative for shooting to work
		targetPos = -(GetPlayerVector());
		float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
		aimAngle = Quaternion.AngleAxis(angle, Vector3.forward);
	}

	protected override void FireProjectile()
	{
		// overridden because not using UpperBody from Actor
		GameObject proj = Instantiate(projectile, transform.position, aimAngle);
		ammo--;
	}

	/// RECIEVING DAMAGE ///

	public override void GetHit()
	{
		hitPoints--;
		if(hitPoints <= 0)
			Die();
	}

	protected override void Die()
	{
		Scoreboard.instance.AddPoints(1);

		EnemySpawner.survivingEnemies--;

		Destroy(gameObject);
	}

}
