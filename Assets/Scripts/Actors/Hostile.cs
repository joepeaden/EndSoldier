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
		// base.Start();
		hitPoints = 3;

		anim = transform.GetChild(0).GetComponent<Animator>();
		
		targetInRange = false;
		
		player = GameObject.FindGameObjectWithTag("Player");

		ChooseSocket();

		gotHitTimer = 1f;
	}

    void Update()
	{
		GameObject target;
		if(targetSocket != null)
			target = targetSocket;
		else
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
				fireRateTimer -= Time.deltaTime;
				
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
		{
			player.GetComponent<Player>().GetHit(1, 0);
		}
	}

	public void TargetInRange(bool inRange)
	{
		targetInRange = inRange; 
	}

	protected override void UpdateAim(Vector2 targetPos)
	{
		// for some reason, playervector needs to be negative for shooting to work
		targetPos = -(GetPlayerVector());
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

	public override void GetHit(int damage, float impact)
	{
		hitPoints -= damage;
		gotHit = true;
		//GetComponent<Rigidbody2D>().AddForce()
		if(hitPoints <= 0)
			Die();

		// apply feedback
		StartCoroutine("GotHitFeedback");
	}

	IEnumerator GotHitFeedback()
    {
		SpriteRenderer renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
	    Color originalColor = renderer.color;

		renderer.color = Color.red;

		// yield return null;
		yield return new WaitForSeconds(0.2f);
		renderer.color = originalColor;
		yield return new WaitForSeconds(0.2f);
		renderer.color = Color.red;
		yield return new WaitForSeconds(0.2f);
		renderer.color = originalColor;
		yield return new WaitForSeconds(0.2f);
		renderer.color = Color.red;
		yield return new WaitForSeconds(0.2f);
		renderer.color = originalColor;
		
	}

	protected override void Die()
	{
		// vacate the socket if occupied
		if(targetSocket != null)
		{
			targetSocket.GetComponent<Socket>().EmptySocket();
			targetSocket = null;
		}

		Scoreboard.instance.AddPoints(1);

		EnemySpawner.survivingEnemies--;

		Destroy(gameObject);
	}
}
