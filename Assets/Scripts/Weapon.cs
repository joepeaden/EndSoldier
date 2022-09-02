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
    protected int ammo = 1;
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

        //Quaternion fireDirection = new Quaternion();
        //fireDirection.eulerAngles = transform.rotation.eulerAngles * (Random.Range(-1, 1) * recoil);

        Instantiate(projectile, projectileSpawnPosition, transform.rotation);//fireDirection);

        attackSound.Play();
        
        if(ammo > 0)
            StartCoroutine("PrepareToAttack");
    }

    // refers to the time in between shots
    private IEnumerator PrepareToAttack()
    {
        yield return new WaitForSeconds(attackInterval);
        readyToAttack = true;
    }

    public void AddAmmo(int ammo)
    {
        this.ammo += ammo;
        StartCoroutine("PrepareToAttack");
    }

    public int GetAmmo()
    {
        return ammo;
    }

    public string GetName()
    {
        return name;
    }
}
