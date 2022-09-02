using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// the player controller.
// things to implement: recoil control value (reduced recoil from weapon)

public class Player : MonoBehaviour
{
	public Text ptstext;
	public Text hptext;
	public Text ammotext;
	public bool reloading;

	public Transform reticle;
	public GameObject laser;

    public Weapon heavyBolter;
	public Weapon bolter;

	[SerializeField]
	private Animator anim;

	[SerializeField]
	private PlayerData data;

	[SerializeField]
	private GameObject upperBody;

	[SerializeField]
	private Weapon weapon;

	private float moveSpeed;
	private int hitPoints;

	new void Start()
	{
		data.hitPoints = 10;
		hitPoints = 10;
        UIManager.instance.UpdateWeaponInfoUI(weapon.GetName(), weapon.GetAmmo());
	}

    void Update()
	{
		//bool firing = false;
		//bool running = false;
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

			//transform.Translate(moveVector * moveSpeed * Time.deltaTime);
			//running = true;
		}
		//else
		//{
		//    anim.SetBool("running", false);
		//}

		if (!sprinting && Input.GetButton("Fire1"))
        {
            if (weapon != null)
            {
                bool ammoInWeapon = weapon.InitiateAttack();
                // if out of ammo, ammoInWeapon will be false
                if (!ammoInWeapon)
                {
                    weapon = bolter;
                }
                UIManager.instance.UpdateWeaponInfoUI(weapon.GetName(), weapon.GetAmmo());
            }
        }

  //      if (firing)
		//{
		//	anim.Play("vort_firing");
		//}
		//else if(running)
		//{
		//	anim.SetBool("running", true);
		//}
		//else
		//{
		//	anim.SetBool("running", false);			
		//	anim.SetBool("firing", false);			
		//}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Pickup")
		{
			ReloadWeapon();
			SwitchWeapons();
			Destroy(other.gameObject);
		}
	}

	private void ReloadWeapon()
	{
		heavyBolter.AddAmmo(50);
	}

	private void SwitchWeapons()
	{
		weapon = heavyBolter;
        UIManager.instance.UpdateWeaponInfoUI(weapon.GetName(), weapon.GetAmmo());
	}

	Vector2 oldAimVector;
	public float rotationTorque;
	protected void UpdateAim(Vector2 aimVector)
	{
		// normalized direction to shoot the projectile
		aimVector = (reticle.position - transform.position).normalized;

		// no idea what this math is.
		float angle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        // doesn't work
        bool crossedZeroDown = rotation.eulerAngles.z > 305 && upperBody.transform.rotation.eulerAngles.z < 45;
        bool crossedZeroUp = rotation.eulerAngles.z < 45 && upperBody.transform.rotation.eulerAngles.z > 305;

		if (Mathf.Abs(rotation.eulerAngles.z - upperBody.transform.rotation.eulerAngles.z) < 3)
		{
			upperBody.GetComponent<Rigidbody2D>().angularVelocity = 0;
			upperBody.GetComponent<Rigidbody2D>().MoveRotation(rotation);
		}
		else if (!crossedZeroDown && rotation.eulerAngles.z > upperBody.transform.rotation.eulerAngles.z || crossedZeroUp)
		{
            upperBody.GetComponent<Rigidbody2D>().AddTorque(rotationTorque);
           // Debug.Log("Current Rotation: " + upperBody.transform.rotation.eulerAngles + " | New Rotation: " + rotation.eulerAngles);
		}
		else
		{
			upperBody.GetComponent<Rigidbody2D>().AddTorque(-rotationTorque);
		}


		oldAimVector = aimVector;

        //upperBody.transform.rotation = rotation;
    }

	public void GetHit(int damage)
	{
		hitPoints -= damage;

		UIManager.instance.UpdateHealthBar(hitPoints, data.hitPoints);

		if(hitPoints <= 0)
			Die();

		StartCoroutine("SpriteColorFlash");
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
