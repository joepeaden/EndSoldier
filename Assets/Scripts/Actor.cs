using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
	public Animator anim;

	public float moveSpeed;
	// public float vertMoveSpeed;

	public GameObject projectile;
	public float fireRate;
	protected float fireRateTimer;

	public GameObject upperBody;

	protected int hitPoints;

	// protected float[] vertMoveLimits; 

	public int ammo;
	public int reserveAmmo;

	protected void Start()
	{
		// vertMoveLimits = new float[2];	
		// vertMoveLimits[0] = -2f; 
		// vertMoveLimits[1] = -4.5f;
		//anim = GetComponent<Animator>();
		fireRateTimer = fireRate;
	}

	protected virtual void FireProjectile()
	{
		GameObject proj = Instantiate(projectile, upperBody.transform.position, upperBody.transform.rotation);
		ammo--;
	}

	protected abstract void UpdateAim(Vector2 targetPos);

	public abstract void GetHit();

	protected abstract void Die();
}
