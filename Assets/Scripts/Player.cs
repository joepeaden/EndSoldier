using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : Actor
{
	public GameObject[] sockets;
	public static int pts;
	public Text ptstext;
	public Text hptext;
	public Text ammotext;
	public bool reloading;
	public GameObject resetButton;
	// joysticks
	public FixedJoystick shootJoystick;
	public FixedJoystick moveJoystick;
	bool shooting;

	new void Start()
	{
		base.Start();
		hitPoints = 3;
		pts = 0;
		ammo = 30;
		reserveAmmo = 900;
		UpdateUI();
		reloading = false;

		// setting joystick to values of strictly 1, 0, or -1
		// moveJoystick.SnapX = true;
		// moveJoystick.SnapY = true;

		// shootJoystick.SnapX = true;
		// shootJoystick.SnapY = true;
	}

	public void UpdateUI()
	{
		ptstext.text = "Points: " + pts;
		hptext.text = "HP: " + hitPoints;
		ammotext.text = "Ammo: " + ammo + " / " + reserveAmmo;
	}

	// clamps value to -1, 0, and 1
	// float ClampN101(float f)
	// {
	// 	if(f > 0)
	// 		return 1;
	// 	else if(f < 0)
	// 		return -1;
	// 	else
	// 		return 0;
	// }

    void Update()
	{
		UpdateUI();

		// make upper body follow mouse
		// UpdateAim();

		bool firing = false;
		bool running = false;

		// float vert = ClampN101(moveJoystick.Vertical);
		// float horiz = ClampN101(moveJoystick.Horizontal);

		if(moveJoystick.Vertical != 0 || moveJoystick.Horizontal != 0) //vert != 0 || horiz != 0)
		{
			Vector2 moveVector = moveJoystick.Direction;//Vector3.Normalize(moveJoystick.Direction);
			transform.Translate(moveVector * moveSpeed);
			UpdateAim(moveVector);
		}

		// // vertical movement
		// if(Input.GetKey(KeyCode.W) || vert > 0)
		// {
		// 	// move up
		// 	transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
		// 	running = true;
		// }
		// else if(Input.GetKey(KeyCode.S) || vert < 0)
		// {
		// 	// move down
		// 	transform.Translate(new Vector3(0, -moveSpeed * Time.deltaTime, 0));
		// 	running = true;
		// } 
		
		// // horizontal movement
		// if(Input.GetKey(KeyCode.D) || horiz > 0)
		// {
		// 	// move right
		// 	transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
		// 	running = true;
		// }
		// else if(Input.GetKey(KeyCode.A) || horiz < 0)
		// {
		// 	// move left
		// 	transform.Translate(new Vector3(-moveSpeed * Time.deltaTime, 0, 0));
		// 	running = true;
		// }
		// else
		// {
		// 	anim.SetBool("running", false);			
		// }

		if(Input.GetButtonDown("Fire2"))
		{
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

				}
				 else
				{
					anim.Play("vort_reloading");
					fireRateTimer = 2f;
					reloading = true;
				}
			}
			else 
			{
				//if(Input.GetButton("Fire1"))// && TapNotInMovementJoystick())
				// if pressing joystick to edge of range
				// if(shootJoystick.Direction.magnitude == shootJoystick.HandleRange)
				if(shooting)
				{
					FireProjectile();
					fireRateTimer = fireRate;
					firing = true;
					shooting = false;
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

	public void Shoot()
	{
		shooting = true;
	}

	bool TapNotInMovementJoystick()
	{
		// Vector2 point = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position
		// if()
		return moveJoystick.Direction == Vector2.zero ? true : false;
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

	protected override void UpdateAim(Vector2 targetPos)
	{
		// for computer
		// Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;

		// Vector2 targetPos = moveJoystick.Direction;

		// for mobile
		// Vector2 targetPos = shootJoystick.Direction;
		// Vector2 targetPos = 

		float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		upperBody.transform.rotation = rotation;
	}

	public override void GetHit()
	{
		hitPoints--;
		if(hitPoints <= 0)
			Die();
	}

	protected override void Die()
	{
		resetButton.SetActive(true);
	}

	public void ResetGame()
	{
		resetButton.SetActive(false);
		SceneManager.LoadSceneAsync(0);
	}

	public GameObject[] GetSockets()
	{
		return sockets;
	} 
}
