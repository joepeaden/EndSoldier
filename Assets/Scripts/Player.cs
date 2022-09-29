using System.Collections;
using UnityEngine;

// the player controller.
// things to implement: recoil control value (reduced recoil from weapon)

public class Player : Actor
{
	[SerializeField]
	private PlayerData data;
	[SerializeField]
	private GameObject upperBody;
	[SerializeField]
	private Weapon weapon;
	[SerializeField]
	private Transform reticle;
	[SerializeField]
	private GameObject laser;

	//// vars local to this class that change ////
	// recoil lerp value - seems like it's gotta be a class var for some reason. Don't feel like looking into it.
	private float t = 0.0f;
	private float movementSpeed;
	private int hitPoints;

	private Rigidbody rb;

	[SerializeField] private GameObject sprite;
	private bool isCrouching;

	private void Start()
	{
		hitPoints = data.hitPoints;

		TestMethod(2);

		rb = GetComponent<Rigidbody>();

		//UIManager.instance.UpdateWeaponInfoUI(weapon.GetName(), weapon.GetAmmo());
	}

	/// <summary>
	/// just for test
	/// </summary>
	/// <param name="p">
	/// a param
	/// </param>
	private void TestMethod(int p)
    {
		p += 1;
		return;
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

		// probably need to handle state better

		// prob not the best way to do this, but fuck it for now
		// check if we're aiming
		if (movementSpeed == data.aimMoveForce)
        {
			laser.SetActive(true);
			reticle.GetComponent<SpriteRenderer>().enabled = true;
		}
		else
        {
			laser.SetActive(false);
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
		UpdateAim(Vector3.zero);

		Vector3 moveVector = GetMoveVector();
		if (moveVector != Vector3.zero)
		{
			rb.AddForce(moveVector * movementSpeed);
		}

		// do we want to have jumping?
		//if (Input.GetKeyDown(KeyCode.Space))
  //      {
		//	rb.AddForce(data.jumpForce);
  //      }
	}

	protected void ToggleCrouch()
    {
		Collider playerCollider = GetComponent<BoxCollider>();
		if (isCrouching)
		{
			transform.localScale = new Vector2(1f, 1f);
			//playerCollider.offset = new Vector2(0, 0);
			//playerCollider. = new Vector2(1, 1);
		}
		else
        {
			transform.localScale = new Vector2(1f, .5f);
			//playerCollider.offset = new Vector2(0, -.25f);
			//playerCollider.size = new Vector2(1, .5f);
		}

		isCrouching = !isCrouching;
	}

    protected void UpdateAim(Vector2 aimVector)
	{
		// normalized direction to shoot the projectile
		aimVector = (reticle.position - transform.position).normalized;

		// no idea what this math is.
		float angle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // if we cross from 360 - 0 or the other way around, handle it
        bool crossedZeroDown = rotation.eulerAngles.z > 180 && upperBody.transform.rotation.eulerAngles.z < 90;
        bool crossedZeroUp = rotation.eulerAngles.z < 90 && upperBody.transform.rotation.eulerAngles.z > 180;

		if (Mathf.Abs(rotation.eulerAngles.z - upperBody.transform.rotation.eulerAngles.z) < 10)
		{
			upperBody.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			upperBody.GetComponent<Rigidbody>().MoveRotation(rotation);
		}
		else if (!crossedZeroDown && rotation.eulerAngles.z > upperBody.transform.rotation.eulerAngles.z || crossedZeroUp)
		{
            upperBody.GetComponent<Rigidbody>().AddTorque(Vector3.forward * data.rotationTorque);
    	}
		else
		{
			upperBody.GetComponent<Rigidbody>().AddTorque(Vector3.forward * -data.rotationTorque);
		}
    }

	public override void GetHit(int damage)
	{
		hitPoints -= damage;

		//UIManager.instance.UpdateHealthBar(hitPoints, data.hitPoints);

		if(hitPoints <= 0)
			Die();
	}

	protected void Die()
	{
        FlowManager.instance.GameOver();
    }

	private Vector3 GetMoveVector()
    {
		Vector3 moveVector = Vector3.zero;
		//if (Input.GetKey(KeyCode.W))
		//{
		//	moveVector += Vector3.up;
		//}
		//if (Input.GetKey(KeyCode.S))
		//{
		//	moveVector += -Vector3.up;
		//}
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

	private void Reload()
    {

    }
}
