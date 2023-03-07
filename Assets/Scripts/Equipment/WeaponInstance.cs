using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Joseph Peaden

/// <summary>
/// Weapon that functions and exists in the game world.
/// </summary>
public class WeaponInstance : MonoBehaviour
{
    const string WEAPON_MODEL_TAG = "WeaponModel";

    // components & child objects
    [SerializeField] private GameObject weaponFlash;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private GameObject aimGlow;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform gunModelParent;
    [SerializeField] private Transform muzzle;

    /// <summary>
    /// Just a debug option.
    /// </summary>
    //[SerializeField] private bool infiniteAmmo;

    // the actor who is using this weapon
    private Actor actorOperator;

    public InventoryWeapon inventoryWeapon;
    private int ammoInWeapon;
    private Vector3 originalLocalPosition;

    // states
    private bool aiming;
    private bool readyToAttack = true;
    private bool reloading;

    // has to do with weapon recoil - honestly confused how it works but right now it has to be class var
    private float t = 0.0f;

    private void Start()
    {
        actorOperator = transform.GetComponentInParent<Actor>();
        actorOperator.OnActorBeginAim += BeginAim;
        actorOperator.OnActorEndAim += EndAim;
        actorOperator.OnCrouch.AddListener(HandleCrouch);
        actorOperator.OnStand.AddListener(HandleStand);

        originalLocalPosition = transform.localPosition;
    }

    private void LateUpdate()
    {
        // this was here to allow actors to aim at crouching enemies. It caused problems though so just removing it for now.

        //Vector3 target = actorOperator.target;//Vector3.right;//transform.worldToLocalMatrix.MultiplyVector(transform.forward);
        //target.y = actorOperator.lookTarget.y;
        //if (actorOperator.target != null && actorOperator.IsAlive)
        //{
        //    transform.LookAt(actorOperator.target);
        //}
        //Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1000f);

        //transform.Rotate(Vector3.right, 1);
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

    public void UpdateWeapon(InventoryWeapon weapon)
    {
        StopAllCoroutines();

        readyToAttack = weapon.amountLoaded > 0;
        reloading = false;

        // save current ammo before swapping
        if (inventoryWeapon != null)
        {
            inventoryWeapon.amountLoaded = ammoInWeapon;
        }

        ammoInWeapon = weapon.amountLoaded;
        audioSource.clip = weapon.data.attackSound;

        // delete only weapon models from the gun parent
        for (int i = 0; i < gunModelParent.childCount; i++)
        {
            if (gunModelParent.GetChild(i).gameObject.CompareTag(WEAPON_MODEL_TAG))
            {
                Destroy(gunModelParent.GetChild(i).gameObject);
            }
        }
        GameObject weapnGO = Instantiate(weapon.data.modelPrefab, gunModelParent);
        weapnGO.tag = WEAPON_MODEL_TAG;

        muzzle.localPosition = weapon.data.muzzlePosition;

        inventoryWeapon = weapon;
    }

    /// <summary>
    /// Cßoroutine that projects a raycast from the weapon's position in the direction it is facing, and moves the aim light to the first collision point.
    /// </summary>
    private IEnumerator ProjectRayCastAndMoveAimGlowToFirstCollision()
    {
        while (aiming)
        {
            RaycastHit hit;
            Ray ray = new Ray(muzzle.position, muzzle.forward);

            int layerMask = LayerMask.GetMask(IgnoreLayerCollisions.CollisionLayers.HouseAndFurniture.ToString(), IgnoreLayerCollisions.CollisionLayers.Actors.ToString(), IgnoreLayerCollisions.CollisionLayers.IgnoreFurniture.ToString(), "PlayerZoneCollider");
            
            if (Physics.Raycast(ray, out hit, int.MaxValue, layerMask))
            {
                aimGlow.transform.position = hit.point;
         
                line.enabled = true;
                line.SetPosition(0, muzzle.position);
                line.SetPosition(1, hit.point);
            }

            yield return null;
        }
    }

    public bool InitiateAttack(float actorRecoilControl, bool triggerPull)
    {
        if (!reloading && (triggerPull || inventoryWeapon.data.isAutomatic))
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
                PlayAudioClip(inventoryWeapon.data.emptyWeaponSound);
            }
        }

        return false;
    }

    private void LaunchAttack(float actorRecoilControl)
    {
        ammoInWeapon--;
        readyToAttack = false;

        // need to put bullet at end of gun barrel so it doesn't hit player
        //Vector3 projectileSpawnPosition = transform.position;
        //projectileSpawnPosition += transform.forward * 1.5f;

        Projectile projectile = Instantiate(inventoryWeapon.data.projectile, muzzle.position, muzzle.rotation).GetComponent<Projectile>();
        projectile.SetOwningActor(actorOperator);

        StopCoroutine(Flash());
        StartCoroutine(Flash());

        PlayAudioClip(inventoryWeapon.data.attackSound);

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
        float maximum = Random.Range(-1, 2) * inventoryWeapon.data.recoil;

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
        yield return new WaitForSeconds(inventoryWeapon.data.attackInterval);
        readyToAttack = true;
    }

    // returns false if no spare ammunition
    public bool StartReload()
    {
        readyToAttack = false;

        if (!reloading)
        {
            StartCoroutine("Reload");
        }

        reloading = true;

        return true;
    }

    private IEnumerator Reload()
    {
        PlayAudioClip(inventoryWeapon.data.reloadSound, 12f);

        if (actorOperator.IsPlayer)
        {
            GameplayUI.Instance.StartReloadBarAnimation(inventoryWeapon.data.reloadTime);
        }

        yield return new WaitForSeconds(inventoryWeapon.data.reloadTime);

        audioSource.Stop();

        // just some mafths. first IF is if there's not enough ammo for a full mag left, second is otherwise.
        int amountNeeded = inventoryWeapon.data.ammoCapacity - ammoInWeapon;
        inventoryWeapon.amount = inventoryWeapon.amount - amountNeeded;
        if (inventoryWeapon.amount < 0)
        {
            inventoryWeapon.amount = 0;
            ammoInWeapon = inventoryWeapon.data.ammoCapacity + inventoryWeapon.amount;
        
        }
        else
        {
            ammoInWeapon = inventoryWeapon.data.ammoCapacity;
        }
            
        // if infinite ammo or it's not the player, don't deplete backup ammo.
        if (inventoryWeapon.data.hasInfiniteBackupAmmo|| !actorOperator.IsPlayer)
        {
            inventoryWeapon.amount = inventoryWeapon.data.totalAmount;
        }

        readyToAttack = true;
        reloading = false;
    }

    private Vector3 originalDimensions;

    private void HandleCrouch()
    {
        transform.localPosition = new Vector3(originalLocalPosition.x, 0.5f, originalLocalPosition.z);
    }

    private void HandleStand()
    {
        transform.localPosition = originalLocalPosition;
    }

    private void PlayAudioClip(AudioClip clip, float timeToPlayAt = 0)
    {
        // don't need to set this slomo every time can just do it in an event once.
        audioSource.pitch = GameManager.isSlowMotion ? GameManager.slowMotionSpeed : 1f;
        audioSource.clip = clip;
        audioSource.time = 0;
        audioSource.Play();
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
        return inventoryWeapon.data.displayName;
    }

    private void OnDestroy()
    {
        if (actorOperator != null)
        {
            actorOperator.OnActorBeginAim -= BeginAim;
            actorOperator.OnActorEndAim -= EndAim;
        }
    }
}
