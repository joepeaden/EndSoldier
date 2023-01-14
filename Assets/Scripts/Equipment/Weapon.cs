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
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject aimGlow;
    [SerializeField] private LineRenderer line;

    // debug options
    [SerializeField] private bool infiniteAmmo;

    // the actor who is using this weapon
    private Actor actorOperator;
    
    private int ammoInWeapon;

    // states
    private bool aiming;
    private bool readyToAttack = true;
    private bool reloading;

    // has to do with weapon recoil - honestly confused how it works but right now it has to be class var
    private float t = 0.0f;

    private void Start()
    {
        ammoInWeapon = data.ammoCapacity;
        audioSource.clip = data.attackSound;

        actorOperator = transform.GetComponentInParent<Actor>();
        actorOperator.OnActorBeginAim += BeginAim;
        actorOperator.OnActorEndAim += EndAim;
        
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        readyToAttack = true;
        reloading = false;
    }

    private void BeginAim()
    {
        aiming = true;
        aimGlow.SetActive(true);
        StartCoroutine(ProjectRayCastAndMoveAimGlowToFirstCollision());
    }
    
    private void EndAim()
    {
        aiming = false;
        line.enabled = false;
        aimGlow.SetActive(false);
        StopCoroutine(ProjectRayCastAndMoveAimGlowToFirstCollision());
    }

    /// <summary>
    /// Cßoroutine that projects a raycast from the weapon's position in the direction it is facing, and moves the aim light to the first collision point.
    /// </summary>
    private IEnumerator ProjectRayCastAndMoveAimGlowToFirstCollision()
    {
        while (aiming)
        {
            RaycastHit hit;
            Ray ray = new Ray(transform.position, transform.forward);

            // get opposite of projectile layer mask
            // int layerMask = ~LayerMask.GetMask("Projectiles");
            int layerMask = LayerMask.GetMask(IgnoreLayerCollisions.CollisionLayers.HouseAndFurniture.ToString(), IgnoreLayerCollisions.CollisionLayers.Actors.ToString(), IgnoreLayerCollisions.CollisionLayers.IgnoreFurniture.ToString());
            
            if (Physics.Raycast(ray, out hit, 1000, layerMask))//, .GetMask("tiles").Projec))
            {
                aimGlow.transform.position = hit.point;
         
                line.enabled = true;
                line.SetPosition(0, transform.position);
                line.SetPosition(1, hit.point);
            }

            yield return null;
        }
    }

    public bool InitiateAttack(float actorRecoilControl, bool triggerPull)
    {
        if (!reloading)
        {
            // wait for gun to cycle (fire rate)
            if (readyToAttack)
            {
                LaunchAttack(actorRecoilControl);
                return true;
            }
            // returning false to indicate to player out of ammo
            else if (triggerPull && ammoInWeapon <= 0)
            {
                PlayAudioClip(data.emptyWeaponSound);
            }
        }

        return false;
    }

    private void LaunchAttack(float actorRecoilControl)
    {
        if(!infiniteAmmo)
            ammoInWeapon--;
        
        readyToAttack = false;

        // need to put bullet at end of gun barrel so it doesn't hit player
        Vector3 projectileSpawnPosition = transform.position;
        projectileSpawnPosition += transform.forward * 1.5f;

        GameObject projectile = Instantiate(data.projectile, projectileSpawnPosition, transform.rotation);
        projectile.GetComponent<Projectile>().SetOwningActor(actorOperator);

        StopCoroutine(Flash());
        StartCoroutine(Flash());

        PlayAudioClip(data.attackSound);

        // never really worked. jsut comment out for now BRO.
        //StopCoroutine(ApplyRecoil(actorRecoilControl));
        //StartCoroutine(ApplyRecoil(actorRecoilControl));
        
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
        PlayAudioClip(data.reloadSound, 12f);

        if (actorOperator.IsPlayer)
        {
            GameplayUI.Instance.StartReloadBarAnimation(data.reloadTime);
        }

        yield return new WaitForSeconds(data.reloadTime);

        audioSource.Stop();

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

    private void PlayAudioClip(AudioClip clip, float timeToPlayAt = 0)
    {
        // don't need to set this slomo every time can just do it in an event once.
        audioSource.pitch = GameManager.isSlowMotion ? GameManager.slowMotionSpeed : 1f;
        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();
    }
}
