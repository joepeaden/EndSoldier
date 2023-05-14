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

    private GameObject weaponModelGameObject;

    /// <summary>
    /// Just a debug option.
    /// </summary>
    //[SerializeField] private bool infiniteAmmo;

    // the actor who is using this weapon
    private Actor actorOperator;
    private Actor ActorOperator
    {
        get
        {
            if (actorOperator == null)
            {
                actorOperator = transform.GetComponentInParent<Actor>();
            }
            return actorOperator;
        }
        set
        {
            actorOperator = value;
        }
    }

    private Vector3 actorVelocity;

    public InventoryWeapon inventoryWeapon;
    private int ammoInWeapon;
    private Vector3 originalLocalPosition;

    // states
    private bool aiming;
    private bool readyToAttack = true;
    private bool reloading;

    [Header("Debug Options")]
    [SerializeField]
    private bool drawAccuracyAngle;

    private void Start()
    {
        ActorOperator.OnActorBeginAim += BeginAim;
        ActorOperator.OnActorEndAim += EndAim;
        ActorOperator.OnCrouch.AddListener(HandleCrouch);
        ActorOperator.OnStand.AddListener(HandleStand);
        ActorOperator.OnDeath.AddListener(HandleDeath);
        ActorOperator.EmitVelocityInfo.AddListener(ReceiveActorVelocityData);
        originalLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        if (drawAccuracyAngle)
        {
            float accuracy = GetAccuracy();

            Quaternion projRot = muzzle.rotation;
            Quaternion ray1Dir = new Quaternion();
            ray1Dir.eulerAngles = new Vector3(projRot.eulerAngles.x, projRot.eulerAngles.y + accuracy, projRot.eulerAngles.z);
            Quaternion ray2Dir = new Quaternion();
            ray2Dir.eulerAngles = new Vector3(projRot.eulerAngles.x, projRot.eulerAngles.y - accuracy, projRot.eulerAngles.z);

            Debug.DrawRay(muzzle.position, ray1Dir * Vector3.forward * 10f, Color.red);
            Debug.DrawRay(muzzle.position, ray2Dir * Vector3.forward * 10f, Color.red);
        }
    }

    private void LateUpdate()
    {
        transform.position = gunModelParent.position;
        transform.rotation = actorOperator.transform.rotation;//gunModelParent.rotation;

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

    private void HandleDeath()
    {
        if (weaponModelGameObject != null)
        {
            weaponModelGameObject.layer = (int)LayerNames.CollisionLayers.IgnoreActors;
            StartCoroutine(DetatchFromActor());
        }
    }

    /// <summary>
    /// Wait just a moment before detatching the weapon from the actor so that the weapon gets a little unique rotation etc.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DetatchFromActor()
    {
        yield return new WaitForSeconds(.2f);
        weaponModelGameObject.transform.parent = null;
        weaponModelGameObject.AddComponent<BoxCollider>();
        weaponModelGameObject.AddComponent<Rigidbody>();
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

        if (ActorOperator.IsPlayer)
        {
            // play reload when switch for cools
            PlayAudioClip(weapon.data.reloadSound);
        }

        ammoInWeapon = weapon.amountLoaded;

        // leaving in case necessary later

        // delete only weapon models from the gun parent
        //for (int i = 0; i < gunModelParent.childCount; i++)
        //{
        //    if (gunModelParent.GetChild(i).gameObject.CompareTag(WEAPON_MODEL_TAG))
        //    {
        //        Destroy(gunModelParent.GetChild(i).gameObject);
        //    }
        //}

        if (weaponModelGameObject != null)
        {
            Destroy(weaponModelGameObject);
            // not sure if this would result from Destroy, so just in case
            weaponModelGameObject = null;
        }
        weaponModelGameObject = Instantiate(weapon.data.modelPrefab, transform);
        weaponModelGameObject.tag = WEAPON_MODEL_TAG;
        weaponModelGameObject.layer = ActorOperator.IsPlayer ? (int)LayerNames.CollisionLayers.PlayerOutline : (int)LayerNames.CollisionLayers.EnemyOutline;

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

            int layerMask = LayerMask.GetMask(LayerNames.CollisionLayers.HouseAndFurniture.ToString(), LayerNames.CollisionLayers.Actors.ToString(), LayerNames.CollisionLayers.IgnoreFurniture.ToString(), "PlayerZoneCollider");
            
            if (Physics.Raycast(ray, out hit, int.MaxValue, layerMask))
            {
                //if (hit.transform.gameObject.tag == "HitBox")
                //{
                //    // messy whatever
                //    //hit.transform.parent.GetComponentInChildren<ActorModel>().gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                //    foreach (SkinnedMeshRenderer r in hit.transform.parent.GetComponentInChildren<ActorModel>().gameObject.GetComponentsInChildren<SkinnedMeshRenderer>())
                //    {
                //        r.gameObject.layer = (int)LayerNames.CollisionLayers.EnemyFill;
                //    }
                //}

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
        // remove as many bullets as are fired
        ammoInWeapon--;// ammoInWeapon > inventoryWeapon.data.projFiredPerShot ? inventoryWeapon.data.projFiredPerShot : ammoInWeapon;

        readyToAttack = false;

        for (int proj = 0; proj < inventoryWeapon.data.projFiredPerShot; proj++)
        {
            float accuracyAngle = GetAccuracy();

            // make the bullet less accurate
            float rotAdjust = Random.Range(-accuracyAngle / 2, accuracyAngle / 2);
            Quaternion projRot = muzzle.rotation;
            projRot.eulerAngles = new Vector3(projRot.eulerAngles.x, projRot.eulerAngles.y + rotAdjust, projRot.eulerAngles.z);

            Projectile projectile = Instantiate(inventoryWeapon.data.projectile, muzzle.position, projRot).GetComponent<Projectile>();
            projectile.Initialize(actorOperator, inventoryWeapon.data, proj);
        }

        StopCoroutine(Flash());
        StartCoroutine(Flash());

        
        if (ammoInWeapon > 0)
            StartCoroutine("PrepareToAttack");
    }

    /// <summary>
    /// determine accuracy (aiming, is the player standing still?)
    /// </summary>
    /// <returns>The angle that projectiles can be fired in.</returns>
    private float GetAccuracy()
    {
        float accuracyAngle = inventoryWeapon.data.accuracyAngle;
        if (aiming)
        {
            accuracyAngle /= inventoryWeapon.data.aimingBoon;
        }
        if (actorVelocity.magnitude > 1f)
        {
            accuracyAngle *= inventoryWeapon.data.movementAccuracyPenalty;
        }

        return accuracyAngle;
    }

    private IEnumerator Flash()
    {
        weaponFlash.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        weaponFlash.SetActive(false);
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

        if (ActorOperator.IsPlayer)
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
        if (inventoryWeapon.data.hasInfiniteBackupAmmo|| !ActorOperator.IsPlayer)
        {
            inventoryWeapon.amount = inventoryWeapon.data.totalAmount;
        }

        readyToAttack = true;
        reloading = false;
    }

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


    private void ReceiveActorVelocityData(Vector3 vel)
    {
        actorVelocity = vel;
    }

    private void OnDestroy()
    {
        ActorOperator.OnActorBeginAim -= BeginAim;
        ActorOperator.OnActorEndAim -= EndAim;
        ActorOperator.EmitVelocityInfo.RemoveListener(ReceiveActorVelocityData);
    }
}
