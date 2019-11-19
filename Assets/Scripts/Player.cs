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
		moveJoystick.SnapX = true;
		moveJoystick.SnapY = true;

		// shootJoystick.SnapX = true;
		// shootJoystick.SnapY = true;
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
		UpdateAim();

		bool firing = false;
		bool running = false;

		// vertical movement
		if(Input.GetKey(KeyCode.W) || moveJoystick.Vertical > 0)
		{
			// move up
			transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
			running = true;
		}
		else if(Input.GetKey(KeyCode.S) || moveJoystick.Vertical < 0)
		{
			// move down
			transform.Translate(new Vector3(0, -moveSpeed * Time.deltaTime, 0));
			running = true;
		} 
		
		// horizontal movement
		if(Input.GetKey(KeyCode.D) || moveJoystick.Horizontal > 0)
		{
			// move right
			transform.Translate(new Vector3(moveSpeed * Time.deltaTime, 0, 0));
			running = true;
		}
		else if(Input.GetKey(KeyCode.A) || moveJoystick.Horizontal < 0)
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
				// if pressing joystick to edge of range
				if(shootJoystick.Direction.magnitude == shootJoystick.HandleRange)
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

	protected override void UpdateAim()
	{
		// for computer
		// Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		
		// for mobile
		Vector2 targetPos = shootJoystick.Direction;

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
