using System.Collections;
using UnityEngine;

// Author: Joseph Peaden

/// <summary>
/// Main player script.
/// </summary>
public class Player : Actor
{
	//[SerializeField] private PlayerData data;
	[SerializeField] private Transform reticle;

	private void Start()
	{
		canBeHit = true;

		hitPoints = data.hitPoints;

		rb = GetComponent<Rigidbody>();
	}

    private void Update()
	{
		bool sprinting = false;

		// slow down and improve aim if we're "aiming"
		if (Input.GetKey(KeyCode.LeftShift))
		{
			movementSpeed = data.sprintMoveForce;
			sprinting = true;
		}
		else if(Input.GetButton("Fire2"))
		{
			movementSpeed = data.aimMoveForce;
		}
		else
        {
			movementSpeed = data.normalMoveForce;
        }

		// if we're 
		if (Input.GetButton("Fire2"))
        {
		 	CameraManager.Instance.FollowTarget(reticle);
		}
        else
        {
			CameraManager.Instance.FollowTarget(transform);
        }

		// probably need to handle state better

		// prob not the best way to do this, but fuck it for now
		// check if we're aiming
		if (movementSpeed == data.aimMoveForce)
        {
			reticle.GetComponent<SpriteRenderer>().enabled = true;
		}
		else
        {
			reticle.GetComponent<SpriteRenderer>().enabled = false;
		}

		if (!sprinting && weapon.HasAmmo() && Input.GetButton("Fire1"))
        {
			if (weapon != null)
			{
				weapon.InitiateAttack(data.recoilControl);
            }
        }

		// need to add feedback so player knows they're out of ammo
		if (Input.GetKeyDown(KeyCode.R))
		{
			weapon.StartReload();
        }

		if (Input.GetKeyDown(KeyCode.C))
        {
			ToggleCrouch();
        }
	}

    private void FixedUpdate()
    {
		// normalized direction to shoot the projectile
		Vector2 aimVector = (reticle.position - transform.position).normalized;
		UpdateAim(aimVector);

		Vector3 moveVector = GetMoveVector();
		if (moveVector != Vector3.zero)
		{
			rb.AddForce(moveVector * movementSpeed);
		}
	}

	protected void ToggleCrouch()
    {
		if (isCrouching)
		{
			gameObject.layer = (int) IgnoreLayerCollisions.CollisionLayers.Default;
			canBeHit = true;
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			// need to add clause to this to check if player is actually in cover, not just crouching.
			// set to InCover layer, ignores collisions with bullets
			gameObject.layer = (int) IgnoreLayerCollisions.CollisionLayers.InCover;

			// is canbehit necessary when the layers ignore collision? Probably not.
			canBeHit = false;
			transform.localScale = new Vector3(1f, .5f, 1f);
		}

		isCrouching = !isCrouching;
	}

	public override bool CanBeHit()
    {
		return canBeHit;
    }

	public override void GetHit(int damage)
	{
		hitPoints -= damage;

		if(hitPoints <= 0)
			Die();
	}

	protected void Die()
	{
        //FlowManager.instance.GameOver();
    }

	private Vector3 GetMoveVector()
    {
		Vector3 moveVector = Vector3.zero;
		if (Input.GetKey(KeyCode.A))
		{
			moveVector += -Vector3.right;
		}
		if (Input.GetKey(KeyCode.D))
		{
			moveVector += Vector3.right;
		}

		return moveVector;
	}
}
