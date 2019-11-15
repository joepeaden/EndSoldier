using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hostile : Actor
{
	bool targetInRange;
	GameObject player;
	GameObject[] sockets;
	public GameObject targetSocket = null;
	bool inTargetZone;
	bool gotHit;
	float gotHitTimer;
	public Quaternion aimAngle;
	public Transform targetZone;

	// might be a good place for an Observer pattern, e.g. notify observers of socket being taken
	// subject: player, observer: hostile
	void ChooseSocket()
	{
		sockets = player.GetComponent<Player>().GetSockets();
		foreach(GameObject s in sockets)
		{
			Socket socket = s.GetComponent<Socket>();

			if(!socket.IsOccupied())
			{
				targetSocket = socket.OccupiedBy(gameObject);
				break;
			}
		}
	}

	public void ReachedTargetZone(bool b)
	{
		inTargetZone = b;
	}

	new void Start()
	{
		base.Start();
		hitPoints = 3;

		anim = transform.GetChild(0).GetComponent<Animator>();
		
		targetInRange = false;
		
		player = GameObject.FindGameObjectWithTag("Player");

		ChooseSocket();

		gotHitTimer = 0.5f;
	}

    void Update()
	{
		GameObject target;
		if(targetSocket != null)
			target = targetSocket;
		else
			target = player;

		UpdateAim();

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
			fireRateTimer -= Time.deltaTime;
			if(targetInRange)
			// if(inTargetZone)
			{
				anim.SetBool("running", false);
				
				if(fireRateTimer <= 0f)
				{
					anim.Play("vort_firing");
					fireRateTimer = fireRate;
					Attack();
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

	private void Attack()
	{
		if(targetInRange)
		// if(inTargetZone)
		{
			player.GetComponent<Player>().GetHit();
		}
	}

	public void TargetInRange(bool inRange)
	{
		targetInRange = inRange; 
	}

	protected override void UpdateAim()
	{
		// for some reason, playervector needs to be negative for shooting to work
		Vector3 targetPos = -(GetPlayerVector());
		targetPos.Normalize();
		targetPos *= 0.25f; 
		targetZone.position = targetPos + transform.position;
	}

	// gets unit vector from this object to player
	public Vector3 GetPlayerVector()
	{
		Vector3 playerVector = transform.position - player.transform.position;
		playerVector = Vector3.Normalize(playerVector);
		return playerVector;
	}

	public override void GetHit()
	{
		hitPoints--;
		gotHit = true;
		//GetComponent<Rigidbody2D>().AddForce()
		if(hitPoints <= 0)
			Die();
	}

	protected override void Die()
	{
		// vacate the socket if occupied
		if(targetSocket != null)
		{
			targetSocket.GetComponent<Socket>().EmptySocket();
			targetSocket = null;
		}

		Player.pts++;

		Destroy(gameObject);
	}
}
