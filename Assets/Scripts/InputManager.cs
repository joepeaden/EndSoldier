using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public float speed;

	public GameObject projectile;
	public float fireRate;
	private float fireRateTimer;

	public GameObject upperBody;

	void Start()
	{
		fireRateTimer = fireRate;
	}

	void Update()
	{
		// make upper body follow mouse
		UpdateUpperBodyAngle();

		// movement
		if(Input.GetKey(KeyCode.D))
		{
			// move right
			transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
		}
		else if(Input.GetKey(KeyCode.A))
		{
			// move left
			transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
		}

		fireRateTimer -= Time.deltaTime;
		// shooting
		if(Input.GetButton("Fire1") && fireRateTimer <= 0f)
		{	
			FireProjectile();
			fireRateTimer = fireRate;
		}
	}

	private void FireProjectile()
	{
		GameObject proj = Instantiate(projectile, upperBody.transform.position, upperBody.transform.rotation);
		// proj.GetComponent<Projectile>().UpdateTeam(1);
	}

	private void UpdateUpperBodyAngle()
	{
		// don't really understand this math
		Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		float angle = Mathf.Atan2(targetPos.y, targetPos.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		upperBody.transform.rotation = rotation;
	}

}
