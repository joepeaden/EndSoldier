using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
	public Animator anim;

	public float moveSpeed;

	public GameObject projectile;
	public float fireRate;
	protected float fireRateTimer;

	public GameObject upperBody;

	protected int hitPoints;

	public int ammo;
	public int reserveAmmo;
	
	public Weapon weapon;

	protected abstract void UpdateAim(Vector2 targetPos);

	public abstract void GetHit(int damage, float impact);

	protected abstract void Die();
}
