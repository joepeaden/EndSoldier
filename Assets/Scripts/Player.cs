using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	private int hitPoints;
	private int maxHitPoints;
	[SerializeField]
	private Weapon weapon;

	private float moveSpeed;

	new void Start()
	{
		maxHitPoints = 10;
		hitPoints = 10;
        UIManager.instance.UpdateWeaponInfoUI(weapon.GetName(), weapon.GetAmmo());
	}

    void Update()
	{
		bool firing = false;
		bool running = false;
		bool sprinting = false;

		// slow down and improve aim if we're "aiming"
		if (Input.GetKey(KeyCode.LeftShift))
		{
			moveSpeed = data.sprintMoveSpeed;
			sprinting = true;
		}
		else if(Input.GetButton("Fire2"))
		{
			moveSpeed = data.aimMoveSpeed;
		}
		else
        {
			moveSpeed = data.normalMoveSpeed;
        }

		// probably need to handle state better

		// prob not the best way to do this, but fuck it for now
		// check if we're aiming
		if (moveSpeed == data.aimMoveSpeed)
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
			transform.Translate(moveVector * moveSpeed * Time.deltaTime);
			running = true;
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

	protected void UpdateAim(Vector2 aimVector)
	{
		// normalized direction to shoot the projectile
		aimVector = (reticle.position - transform.position).normalized;
		
		// no idea what this math is.
		float angle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		
		upperBody.transform.rotation = rotation;
	}

	public void GetHit(int damage)
	{
		hitPoints -= damage;

		UIManager.instance.UpdateHealthBar(hitPoints, maxHitPoints);

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
