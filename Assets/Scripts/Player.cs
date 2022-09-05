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


	// recoil lerp value - seems like it's gotta be a class var for some reason. Don't feel like looking into it.
	private float t = 0.0f;

	// should eventually probably move to PlayerData SO - just not sure how it will work out right now
	public float rotationTorque;

	private float moveSpeed;
	private int hitPoints;

	public float recoilControl;

	private void Start()
	{
		hitPoints = data.hitPoints;

		//UIManager.instance.UpdateWeaponInfoUI(weapon.GetName(), weapon.GetAmmo());
	}

    private void Update()
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

		if (!sprinting && weapon.HasAmmo() && Input.GetButton("Fire1"))
        {
			Debug.Log(weapon.GetAmmo());

			if (weapon != null)
			{
				bool ammoInWeapon = weapon.InitiateAttack();

				// this should probably be in weapon class actually
				if (ammoInWeapon)
				{
					StopCoroutine("ApplyRecoil");
					StartCoroutine("ApplyRecoil");
				}
                //UIManager.instance.UpdateWeaponInfoUI(weapon.GetName(), weapon.GetAmmo());
            }
        }

		// need to add feedback so player knows they're out of ammo
		if (Input.GetKeyDown(KeyCode.R))
		{
			Debug.Log("Reloading");
			weapon.StartReload();
        }
	}

	// should probably be in weapon class
	protected IEnumerator ApplyRecoil()
    {

		Transform weaponT = weapon.GetComponent<Transform>();

        bool recoilComplete = false;
		bool returning = false;

		float minimum = 0;
		float maximum = Random.Range(-1, 2) * weapon.recoil;

		do
		{
			Quaternion q = new Quaternion();
			q.eulerAngles = new Vector3(0, 0, Mathf.Lerp(minimum, maximum, t));

			weaponT.localRotation = q;

			t += recoilControl * Time.deltaTime;

			if (t > 1.0f)
			{  
				if (returning)
					recoilComplete = true;

				float temp = maximum;
				maximum = minimum;
				minimum = temp;
				t = 0.0f;
				returning = true;
			}

			yield return null;
		} while (!recoilComplete);

		weaponT.localRotation = Quaternion.identity;

		yield return null;
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

		Debug.Log(hitPoints);

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

	private void Reload()
    {

    }
}
