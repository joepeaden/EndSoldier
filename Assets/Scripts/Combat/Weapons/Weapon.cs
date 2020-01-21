using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Joseph Peaden
// Purpose: Represents all types of weapons, ranged or melee
//          Player and AI should be able to use this

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    protected Projectile projectile;
    protected float fireRate;
    protected int ammo;

    [SerializeField]
	private AudioSource fireWeaponSound;
    private bool bulletInChamber;

    public void Start()
    {
        bulletInChamber = true;
        Initialize();
    }

    public void PullTrigger()
    {
        if(bulletInChamber)
            Fire();
    }

    private void Fire()
    {
        bulletInChamber = false;

        // need to put bullet at end of gun barrel so it doesn't hit player
        Vector3 bulletSpawnPosition = transform.position;
        bulletSpawnPosition += transform.right * 1.5f;
        
        Instantiate(projectile, bulletSpawnPosition, transform.rotation);
        
        fireWeaponSound.Play();
        StartCoroutine("LoadNewBullet");
    }

    // need to think of a better name
    // refers to the time in between shots
    private IEnumerator LoadNewBullet()
    {
        yield return new WaitForSeconds(fireRate);
        bulletInChamber = true;
    }

    protected abstract void Initialize();

}
