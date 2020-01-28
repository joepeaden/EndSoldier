using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Joseph Peaden
// Purpose: Represents all types of weapons, ranged or melee
//          Player and AI should be able to use this

public class Weapon : MonoBehaviour
{
    [SerializeField]
    protected Projectile projectile;
    [SerializeField]
    protected float attackRate;
    // infinite ammo weapons need at least one shot
    [SerializeField]
    protected int ammo = 1;
    [SerializeField]
    protected bool infiniteAmmo;
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
        if(ammo == 0)
           return false;

        return true;
    }

    private void LaunchAttack()
    {
        if(!infiniteAmmo)
            ammo--;
        
        readyToAttack = false;

        // need to put bullet at end of gun barrel so it doesn't hit player
        Vector3 projectileSpawnPosition = transform.position;
        projectileSpawnPosition += transform.right * 1.5f;
        
        Instantiate(projectile, projectileSpawnPosition, transform.rotation);
        
        attackSound.Play();
        
        if(ammo > 0)
            StartCoroutine("PrepareToAttack");
    }

    // refers to the time in between shots
    private IEnumerator PrepareToAttack()
    {
        yield return new WaitForSeconds(attackRate);
        readyToAttack = true;
    }

    public void AddAmmo(int ammo)
    {
        this.ammo += ammo;
        StartCoroutine("PrepareToAttack");
    }

    // protected void Initialize();

}
