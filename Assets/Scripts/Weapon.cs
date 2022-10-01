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

    private float t = 0.0f;

    private bool reloading;

    [SerializeField] protected GameObject weaponFlash;

    public void Start()
    {
        readyToAttack = true;
        // Initialize();
    }

    public bool InitiateAttack(float actorRecoilControl)
    {
        // wait for gun to cycle (fire rate)
        if(readyToAttack)
            LaunchAttack(actorRecoilControl);

        // returning false to indicate to player out of ammo
        if(ammoInWeapon == 0)
           return false;

        return true;
    }

    private void LaunchAttack(float actorRecoilControl)
    {
        if(!infiniteAmmo)
            ammoInWeapon--;
        
        readyToAttack = false;

        // need to put bullet at end of gun barrel so it doesn't hit player
        Vector3 projectileSpawnPosition = transform.position;
        projectileSpawnPosition += transform.right * 1.5f;

        Instantiate(projectile, projectileSpawnPosition, transform.rotation);

        StopCoroutine(Flash());
        StartCoroutine(Flash());

        attackSound.Play();

        StopCoroutine(ApplyRecoil(actorRecoilControl));
        StartCoroutine(ApplyRecoil(actorRecoilControl));
        
        if (ammoInWeapon > 0)
            StartCoroutine("PrepareToAttack");
    }

    protected IEnumerator Flash()
    {
        weaponFlash.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        weaponFlash.SetActive(false);
    }

    // should probably be in weapon class
    protected IEnumerator ApplyRecoil(float actorRecoilControl)
    {
        bool recoilComplete = false;
        bool returning = false;

        float minimum = 0;
        float maximum = Random.Range(-1, 2) * recoil;

        // COULD USE Quaternion.Slerp()

        do
        {
            Quaternion q = new Quaternion();
            q.eulerAngles = new Vector3(0, 0, Mathf.Lerp(minimum, maximum, t));

            transform.localRotation = q;

            t += actorRecoilControl * Time.deltaTime;

            if (t > 1.0f)
            {
                if (returning)
                    recoilComplete = true;

                float temp = maximum;
                maximum = minimum;
                minimum = temp;
                t = 0.0f;
                returning = true;
            }

            yield return null;
        } while (!recoilComplete);

        transform.localRotation = Quaternion.identity;

        yield return null;
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

        if (!reloading)
        {
            StartCoroutine("Reload");
        }

        reloading = true;

        return true;
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(2);

        ammoInWeapon = ammoCapacity;
        readyToAttack = true;
        reloading = false;
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
