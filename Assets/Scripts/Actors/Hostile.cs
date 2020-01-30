using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostile : Actor
{
	bool targetInRange;
	GameObject player;
	// // public GameObject targetSocket = null;
	bool inTargetZone;
	bool gotHit;
	float gotHitTimer;
	public Quaternion aimAngle;
	public Transform targetZone;

	new void Start()
	{
		// base.Start();
		hitPoints = 3;

		anim = transform.GetChild(0).GetComponent<Animator>();
		
		targetInRange = false;
		
		player = GameObject.FindGameObjectWithTag("Player");

		// ChooseSocket();

		gotHitTimer = 1f;
	}

    void Update()
	{
		GameObject target;
		// if(targetSocket != null)
		// 	target = targetSocket;
		// else
		target = player;

		UpdateAim(Vector2.zero);

		if(gotHit)
		{
			gotHitTimer -= Time.deltaTime;

			if(gotHitTimer <= 0f)
			{
				gotHitTimer = 0.5f;
				gotHit = false;
			}
		}
		else
		{
			if(targetInRange)
			{
				anim.SetBool("running", false);
				if(weapon != null)
				{
					anim.Play("vort_firing");
					bool ammoInWeapon = weapon.InitiateAttack();
					// if out of ammo, ammoInWeapon will be false
					if(!ammoInWeapon) {
						;
					}
				}				
			}
			else
			{
				// left and right movement
				if(target.transform.position.x < transform.position.x)
				{
					transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0, 0));
					anim.SetBool("running", true);
				}
				else if(target.transform.position.x > transform.position.x)
				{
					transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
					anim.SetBool("running", true);
				}

				// Up and down movement
				if(target.transform.position.y < transform.position.y)
				{
					transform.Translate(new Vector3(0, -moveSpeed * Time.deltaTime, 0));
					anim.SetBool("running", true);
				}
				else if(target.transform.position.y > transform.position.y)
				{
					transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
					anim.SetBool("running", true);
				}
			}
		}
	}

	public void ReachedTargetZone(bool b)
	{
		inTargetZone = b;
	}

	// private void Attack()
	// {
	// 	if(targetInRange)
	// 	{
	// 		if(weapon != null)
	// 		{
	// 			bool ammoInWeapon = weapon.InitiateAttack();
	// 			// if out of ammo, ammoInWeapon will be false
	// 			if(!ammoInWeapon) {
	// 				;
	// 			}
	// 		}
	// 		// player.GetComponent<Player>().GetHit(1);
	// 	}
	// }

	public void TargetInRange(bool inRange)
	{
		targetInRange = inRange; 
	}

	protected override void UpdateAim(Vector2 targetPos)
	{
		// code for aiming of weapon
		// for some reason, playervector needs to be negative for shooting to work
		targetPos = -(GetPlayerVector());
		float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		upperBody.transform.rotation = rotation;

		// code for melee range zone - should be updated when creating shooting enemies
		// 1. perhaps a raycast to see if the ray hits player, and check magnitude of ray to see if 
		// 	  player is within range of this weapon
		// 2. if that's too hard, can use a collider of various lengths attatched to the end
		//    of the weapon to detect player in range 
		targetPos.Normalize();
		targetPos *= 0.25f; 
		targetZone.position = targetPos + (Vector2)transform.position;
	}

	// gets unit vector from this object to player
	public Vector3 GetPlayerVector()
	{
		Vector3 playerVector = transform.position - player.transform.position;
		playerVector = Vector3.Normalize(playerVector);
		return playerVector;
	}

	public override void GetHit(int damage)
	{
		hitPoints -= damage;
		gotHit = true;
		if(hitPoints <= 0)
			Die();

		// apply feedback
		StartCoroutine("SpriteColorFlash");
	}

	protected override void Die()
	{
		// vacate the socket if occupied
		// if(targetSocket != null)
		// {
		// 	targetSocket.GetComponent<Socket>().EmptySocket();
		// 	targetSocket = null;
		// }

		Scoreboard.instance.AddPoints(1);

		EnemySpawner.survivingEnemies--;

		Destroy(gameObject);
	}
}
