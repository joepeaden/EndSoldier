using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Joseph Peaden
// Purpose: should represent all types of weapons, ranged or melee, but for now coding for ranged and will abstract later
//          Player and AI should be able to use this

public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected Projectile projectile;

    [SerializeField]
    public float recoil;

    // how long it takes between attacks with this weapon
    [SerializeField]
    protected float attackInterval;

    // infinite ammo weapons need at least one shot
    [SerializeField]
    protected int ammoInWeapon = 1;
    [SerializeField]
    int ammoCapacity;

    [SerializeField]
    protected bool infiniteAmmo;
    [SerializeField]
    protected string name;

    [SerializeField]
	private AudioSource attackSound;
    private bool readyToAttack;


    public void Start()
    {
        readyToAttack = true;
        // Initialize();
    }

    public bool InitiateAttack()
    {
        // wait for gun to cycle (fire rate)
        if(readyToAttack)
            LaunchAttack();

        // returning false to indicate to player out of ammo
        if(ammoInWeapon == 0)
           return false;

        return true;
    }

    private void LaunchAttack()
    {
        if(!infiniteAmmo)
            ammoInWeapon--;
        
        readyToAttack = false;

        // need to put bullet at end of gun barrel so it doesn't hit player
        Vector3 projectileSpawnPosition = transform.position;
        projectileSpawnPosition += transform.right * 1.5f;

        Instantiate(projectile, projectileSpawnPosition, transform.rotation);

        attackSound.Play();
        
        if(ammoInWeapon > 0)
            StartCoroutine("PrepareToAttack");
    }

    // refers to the time in between shots
    private IEnumerator PrepareToAttack()
    {
        yield return new WaitForSeconds(attackInterval);
        readyToAttack = true;
    }

    // returns false if no spare ammunition
    public bool StartReload()
    {
        readyToAttack = false;
        ammoInWeapon = 0;

        StartCoroutine("Reload");

        return true;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(2);

        ammoInWeapon = ammoCapacity;
        readyToAttack = true;

        Debug.Log("Finished Reloading!");
    }

    public void AddAmmo(int ammo)
    {
        this.ammoInWeapon += ammo;
        StartCoroutine("PrepareToAttack");
    }

    public int GetAmmo()
    {
        return ammoInWeapon;
    }

    public bool HasAmmo()
    {
        return ammoInWeapon > 0;
    }

    public string GetName()
    {
        return name;
    }
}
