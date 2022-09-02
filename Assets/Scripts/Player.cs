using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// the player controller.
// things to implement: recoil control value (reduced recoil from weapon)

public class Player : MonoBehaviour
{
	public Transform reticle;
	public GameObject laser;

	public Weapon primaryWeapon;

	[SerializeField]
	private PlayerData data;

	[SerializeField]
	private GameObject upperBody;

	[SerializeField]
	private Weapon weapon;

	// should eventually probably move to PlayerData SO - just not sure how it will work out right now
	public float rotationTorque;

	private float moveSpeed;
	private int hitPoints;

	new void Start()
	{
		hitPoints = data.hitPoints;
        //UIManager.instance.UpdateWeaponInfoUI(weapon.GetName(), weapon.GetAmmo());
	}

    void Update()
	{
		bool sprinting = false;

		// slow down and improve aim if we're "aiming"
		if (Input.GetKey(KeyCode.LeftShift))
		{
			moveSpeed = data.sprintMoveForce;
			sprinting = true;
		}
		else if(Input.GetButton("Fire2"))
		{
			moveSpeed = data.aimMoveForce;
		}
		else
        {
			moveSpeed = data.normalMoveForce;
        }

		// probably need to handle state better

		// prob not the best way to do this, but fuck it for now
		// check if we're aiming
		if (moveSpeed == data.aimMoveForce)
        {
			laser.SetActive(true);
			reticle.GetComponent<SpriteRenderer>().enabled = true;
		}
		else
        {
			laser.SetActive(false);
			reticle.GetComponent<SpriteRenderer>().enabled = false;
		}

		UpdateAim(Vector3.zero);

		Vector3 moveVector = GetMoveVector();
		if (moveVector != Vector3.zero)
		{
			GetComponent<Rigidbody2D>().AddForce(moveVector * moveSpeed);
		}

		if (!sprinting && Input.GetButton("Fire1"))
        {
            if (weapon != null)
            {
                bool ammoInWeapon = weapon.InitiateAttack();

                ApplyRecoil();

                //upperBody.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1, 1) * weapon.recoil);

                // if out of ammo, ammoInWeapon will be false
                if (!ammoInWeapon)
                {
                    weapon = primaryWeapon;
                }


                //UIManager.instance.UpdateWeaponInfoUI(weapon.GetName(), weapon.GetAmmo());
            }
        }
	}

    protected void ApplyRecoil()
    {
        weapon.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-1, 1) * weapon.recoil);
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

		if (Mathf.Abs(rotation.eulerAngles.z - upperBody.transform.rotation.eulerAngles.z) < 3)
		{
			upperBody.GetComponent<Rigidbody2D>().angularVelocity = 0;
			upperBody.GetComponent<Rigidbody2D>().MoveRotation(rotation);
		}
		else if (!crossedZeroDown && rotation.eulerAngles.z > upperBody.transform.rotation.eulerAngles.z || crossedZeroUp)
		{
            upperBody.GetComponent<Rigidbody2D>().AddTorque(rotationTorque);
    	}
		else
		{
			upperBody.GetComponent<Rigidbody2D>().AddTorque(-rotationTorque);
		}
    }

	public void GetHit(int damage)
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
		if (Input.GetKey(KeyCode.W))
		{
			moveVector += Vector3.up;
		}
		if (Input.GetKey(KeyCode.S))
		{
			moveVector += -Vector3.up;
		}
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
