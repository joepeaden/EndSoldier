using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Joseph Peaden

/// <summary>
/// Should represent all types of weapons, ranged or melee. Any actor should be able to use this
/// </summary>
public class Weapon : MonoBehaviour
{
    // components & child objects
    [SerializeField] private WeaponData data;
    [SerializeField] private GameObject weaponFlash;
    [SerializeField] private AudioSource attackAudioSource;

    [SerializeField] private bool infiniteAmmo;

    private int ammoInWeapon;

    // states
    private bool readyToAttack = true;
    private bool reloading;

    // has to do with weapon recoil - honestly confused how it works but right now it has to be class var
    private float t = 0.0f;

    private void Start()
    {
        ammoInWeapon = data.ammoCapacity;
        attackAudioSource.clip = data.attackSound;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        readyToAttack = true;
        reloading = false;
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
        projectileSpawnPosition += transform.forward * 1.5f;

        Instantiate(data.projectile, projectileSpawnPosition, transform.rotation);

        StopCoroutine(Flash());
        StartCoroutine(Flash());

        attackAudioSource.Play();

        StopCoroutine(ApplyRecoil(actorRecoilControl));
        StartCoroutine(ApplyRecoil(actorRecoilControl));
        
        if (ammoInWeapon > 0)
            StartCoroutine("PrepareToAttack");
    }

    private IEnumerator Flash()
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
        float maximum = Random.Range(-1, 2) * data.recoil;

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
        yield return new WaitForSeconds(data.attackInterval);
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

        ammoInWeapon = data.ammoCapacity;
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
        return data.displayName;
    }
}
