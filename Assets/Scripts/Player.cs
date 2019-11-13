﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Actor
{
	public GameObject[] sockets;
	public static int pts;
	public Text ptstext;
	public Text hptext;
	public Text ammotext;
	public bool reloading;

	new void Start()
	{
		base.Start();
		hitPoints = 3;
		pts = 0;
		ammo = 30;
		reserveAmmo = 90;
		UpdateUI();
		reloading = false;
	}

	public void UpdateUI()
	{
		ptstext.text = "Points: " + pts;
		hptext.text = "HP: " + hitPoints;
		ammotext.text = "Ammo: " + ammo + " / " + reserveAmmo;
	}

    void Update()
	{
		UpdateUI();

		// make upper body follow mouse
		UpdateUpperBodyAngle();

		bool firing = false;
		bool running = false;

		// vertical movement
		if(Input.GetKey(KeyCode.W))// && transform.position.y <= vertMoveLimits[0])
		{
			// move up
			transform.Translate(new Vector3(0, vertMoveSpeed * Time.deltaTime, 0));
			running = true;
		}
		else if(Input.GetKey(KeyCode.S))// && transform.position.y > vertMoveLimits[1])
		{
			// move down
			transform.Translate(new Vector3(0, -vertMoveSpeed * Time.deltaTime, 0));
			running = true;
		} 
		
		// horizontal movement
		if(Input.GetKey(KeyCode.D))
		{
			// move right
			transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
			running = true;
		}
		else if(Input.GetKey(KeyCode.A))
		{
			// move left
			transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0, 0));
			running = true;
		}
		else
		{
			anim.SetBool("running", false);			
			Debug.Log("Stopping");
		}

		if(Input.GetButtonDown("Fire2"))
		{
			Debug.Log(MeleeZone.targets.Count);
			foreach(Actor a in MeleeZone.targets)
				a.GetHit();
		}

		fireRateTimer -= Time.deltaTime;
		// shooting
		if(fireRateTimer <= 0f)
		{	
			if(reloading == true)
			{
				ammo = 15;
				reserveAmmo -= 15;
				reloading = false;
				fireRateTimer = fireRate;
				anim.SetBool("reloading", false);

			}
			else if(ammo <= 0)
			{
				if(reserveAmmo <= 0)
				{
					Debug.Log("Out Of Ammo");

				}
				 else
				{
					anim.Play("vort_reloading");
					Debug.Log("Reload");
					fireRateTimer = 2f;
					reloading = true;
				}
			}
			else 
			{
				if(Input.GetButton("Fire1"))
				{
					FireProjectile();
					fireRateTimer = fireRate;
					firing = true;
				}
			}
		}

		if(firing)
		{
			anim.Play("vort_firing");//SetBool("firing", true);			
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
		if(other.gameObject.tag == "Stockpile")
		{
			if(reserveAmmo < 120)
				reserveAmmo += 30;

			Destroy(other.gameObject);
		}
	}

	protected override void UpdateUpperBodyAngle()
	{
		// don't really understand this math
		Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		upperBody.transform.rotation = rotation;
	}

	public override void GetHit()
	{
		Debug.Log("Got hit");
		hitPoints--;
		if(hitPoints <= 0)
			Die();
	}

	protected override void Die()
	{
		Debug.Log("Ded");
		Destroy(gameObject);
	}

	public GameObject[] GetSockets()
	{
		return sockets;
	} 
}
