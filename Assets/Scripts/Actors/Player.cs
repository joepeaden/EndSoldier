using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Actor
{
	public GameObject[] sockets;
	public Text ptstext;
	public Text hptext;
	public Text ammotext;
	public bool reloading;
	// joysticks
	public FixedJoystick shootJoystick;
	public FixedJoystick moveJoystick;
	
	public Weapon heavyBolter;
	public Weapon bolter;

	new void Start()
	{
		// base.Start();
		hitPoints = 3;		
		UpdateUI();
	}

	public void UpdateUI()
	{
		// ptstext.text = "Points: " + pts;
		hptext.text = "HP: " + hitPoints;
		// ammotext.text = "Ammo: " + ammo + " / " + reserveAmmo;
	}

    void Update()
	{
		UpdateUI();

		if(shootJoystick.Direction != Vector2.zero)
			// make upper body follow mouse
			UpdateAim(Vector2.zero);

		bool firing = false;
		bool running = false;

	
		if(moveJoystick.Vertical != 0 || moveJoystick.Horizontal != 0) //vert != 0 || horiz != 0)
		{
			Vector2 moveVector = moveJoystick.Direction;//Vector3.Normalize(moveJoystick.Direction);
			transform.Translate(moveVector * moveSpeed);
			running = true;
			// UpdateAim(moveVector);
		} 
		else
		{
			anim.SetBool("running", false);			
		}

		// if(Input.GetButtonDown("Fire2"))
		// {
		// 	foreach(Actor a in MeleeZone.targets)
		// 		a.GetHit();
		// }

		if(shootJoystick.Direction.magnitude == shootJoystick.HandleRange)
		{
			if(weapon != null)
			{
				bool ammoInWeapon = weapon.PullTrigger();
				// if out of ammo, ammoInWeapon will be false
				if(!ammoInWeapon) {
					weapon = bolter;
				}
			}
		}

		if(firing)
		{
			anim.Play("vort_firing");
		}
		else if(running)
		{
			anim.SetBool("running", true);
		}
		else
		{
			anim.SetBool("running", false);			
			anim.SetBool("firing", false);			
		}
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
	}

	protected override void UpdateAim(Vector2 targetPos)
	{
		targetPos = shootJoystick.Direction;
		float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		upperBody.transform.rotation = rotation;
	}

	public override void GetHit(int damage, float impact)
	{
		hitPoints -= damage;
		if(hitPoints <= 0)
			Die();
	}

	protected override void Die()
	{
		FlowManager.instance.GameOver();
	}

	public GameObject[] GetSockets()
	{
		return sockets;
	} 
}
