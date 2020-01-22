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
    // infinite ammo weapons need at least one shot
    protected int ammo = 1;
    protected bool infiniteAmmo;

    [SerializeField]
	private AudioSource fireWeaponSound;
    private bool bulletInChamber;

    public void Start()
    {
        bulletInChamber = true;
        Initialize();
    }

    public bool PullTrigger()
    {
        // wait for gun to cycle (fire rate)
        if(bulletInChamber)
            Fire();

        // returning false to indicate to player out of ammo
        if(ammo == 0)
           return false;

        return true;
    }

    private void Fire()
    {
        if(!infiniteAmmo)
            ammo--;
        
        bulletInChamber = false;

        // need to put bullet at end of gun barrel so it doesn't hit player
        Vector3 bulletSpawnPosition = transform.position;
        bulletSpawnPosition += transform.right * 1.5f;
        
        Instantiate(projectile, bulletSpawnPosition, transform.rotation);
        
        fireWeaponSound.Play();
        
        if(ammo > 0)
            StartCoroutine("LoadNewBullet");
    }

    // refers to the time in between shots
    private IEnumerator LoadNewBullet()
    {
        yield return new WaitForSeconds(fireRate);
        bulletInChamber = true;
    }

    public void AddAmmo(int ammo)
    {
        this.ammo += ammo;
        StartCoroutine("LoadNewBullet");
    }

    protected abstract void Initialize();

}
