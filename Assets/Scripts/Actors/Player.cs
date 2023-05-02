using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Controller for the player, also handles a few player specific things like death.
/// </summary>
/// <remarks>
/// Honestly I kind of feel like input should be handled in its own class. Because then it would be very easy to find, understand, debug.
/// But I don't feel like it at the moment.
/// </remarks>
public class Player : MonoBehaviour
{
	public UnityEvent<InventoryWeapon> OnSwitchWeapons = new UnityEvent<InventoryWeapon>();
	public UnityEvent<Equipment> OnUpdateEquipment = new UnityEvent<Equipment>();
	public UnityEvent OnPlayerDeath = new UnityEvent();

	private Actor actor;
	private Transform reticle;
	private bool triggerPull;

	// control stuff
	private PlayerControls controls;
	private Vector2 movementInput;

	private void Awake()
	{
		actor = GetComponent<Actor>();
		actor.team = Actor.ActorTeam.Friendly;

		controls = new PlayerControls();
		// Add these to on destroy please please please please I beg you
		controls.Gameplay.Move.performed += HandleMovementInput; 
		controls.Gameplay.Move.canceled += ZeroMovementInput;
		controls.Gameplay.Sprint.performed += HandleSprintStart;
		controls.Gameplay.Sprint.canceled += HandleSprintStop;
		controls.Gameplay.Rotate.performed += HandleRotationInput;
		controls.Gameplay.Rotate.canceled += ZeroRotationInput;

	}

	private void Start()
	{
		actor.OnDeath.AddListener(HandlePlayerDeath);
		actor.OnGetHit.AddListener(HandleGetHit);
		actor.OnHeal.AddListener(HandleHeal);
		actor.GetInventory().SetWeaponFromData();

		reticle = GameManager.Instance.GetReticleGO()?.transform;
	}

    private void OnEnable()
    {
		controls.Gameplay.Enable();
    }

	private void OnDisable()
	{
		controls.Gameplay.Disable();
	}

	private void Update()
	{
		if (!GameplayUI.Instance || !GameplayUI.Instance.InMenu())
		{
			if (Input.GetKey(KeyCode.LeftShift))
			{
				//actor.SetState(Actor.State.Sprinting);
			}
			else if (Input.GetButton("Fire2") && !actor.state[Actor.State.Sprinting])
			{
				actor.SetState(Actor.State.Aiming);
				actor.OnActorBeginAim.Invoke();
			}
			else
			{
				//actor.OnActorEndAim.Invoke();
				//actor.SetState(Actor.State.Walking);
			}

			if (Input.GetButtonDown("Fire1"))
			{
				triggerPull = true;
			}
			else
			{
				triggerPull = false;
			}

			if (Input.GetButton("Fire1") && !actor.state[Actor.State.Sprinting])
			{
				actor.AttemptAttack(triggerPull);
			}

			if (Input.GetKeyDown(KeyCode.R))
			{
				actor.AttemptReload();
			}

			if (Input.GetKeyDown(KeyCode.C))
			{
				actor.ToggleCrouch();
			}

			if (Input.GetKeyDown(KeyCode.E))
			{
				actor.AttemptInteraction();

				// hmmmmm I haven't figured out how to handle swapping equipment, etc. Or even if I will allow that.
				OnUpdateEquipment.Invoke(actor.GetInventory().GetEquipment());
			}

			if (Input.GetKeyDown(KeyCode.Q))
			{
				actor.AttemptUseEquipment();
				OnUpdateEquipment.Invoke(actor.GetInventory().GetEquipment());
			}

			if (Input.GetKeyDown(KeyCode.Space) && actor.GetInventory().weaponCount > 1)
			{
				bool result = actor.AttemptSwitchWeapons();
				if (result == true)
				{
					OnSwitchWeapons.Invoke(actor.GetEquippedWeapon());
				}
			}
		}
	}

	private void FixedUpdate()
	{
		if (!GameplayUI.Instance || !GameplayUI.Instance.InMenu())
		{

			float newRotation = Mathf.Atan(rotation.x / rotation.y) * Mathf.Rad2Deg;

			if (rotation.y < 0)
            {
				newRotation -= 180;
            }

            // get the move direction
            Vector3 rotDir = new Vector3(0f, newRotation, 0f);

			Quaternion tRot = transform.rotation;
			tRot.eulerAngles = rotDir;

			tRot = Quaternion.AngleAxis(45, Vector3.up) * tRot;
			transform.rotation = tRot;

            if (actor.state[Actor.State.Sprinting])
			{
				// only go forward if sprinting
				actor.Move(transform.forward, false);
			}
			else
			{
				// get the move direction
                Vector3 moveDir = new Vector3(movementInput.x, 0f, movementInput.y);
				// adjust it for isometric camera pos
				moveDir = Quaternion.AngleAxis(45, Vector3.up) * moveDir;

				moveDir = Vector3.ClampMagnitude(moveDir, 1f);

                actor.Move(moveDir, false);
			}
		}
	}

	private void OnDestroy()
	{
		actor.OnDeath.RemoveListener(HandlePlayerDeath);
		actor.OnGetHit.RemoveListener(HandleGetHit);
		actor.OnHeal.RemoveListener(HandleHeal);
	}

	///////////////
	#region Input
	///////////////
	
	private void HandleMovementInput(InputAction.CallbackContext cntxt)
	{
		movementInput = cntxt.ReadValue<Vector2>();
	}

	private void ZeroMovementInput(InputAction.CallbackContext cntxt)
	{
		movementInput = Vector2.zero;
	}

	private void HandleSprintStart(InputAction.CallbackContext cntxt)
	{
		actor.SetState(Actor.State.Sprinting);
		actor.OnActorEndAim.Invoke();
	}

	private void HandleSprintStop(InputAction.CallbackContext cntxt)
	{
		actor.SetState(Actor.State.Walking);
	}

	private Vector2 rotation;
	public float rotvalue;
	private void HandleRotationInput(InputAction.CallbackContext cntxt)
	{
		rotation = cntxt.ReadValue<Vector2>();
	}

	private void ZeroRotationInput(InputAction.CallbackContext cntxt)
	{
		//rotation = Vector2.zero;
	}

	#endregion

	public Inventory GetInventory()
	{
		return actor.GetInventory();
	}

	/// <summary>
	/// Get the ammo and max ammo of the player's weapon
	/// </summary>
	/// <returns>2 integers: the weapon's loaded ammo, and the total backup ammo. If infinite backup, backup val will be int.MinValue</returns>
	public (int, int) GetAmmo()
	{
		int totalAmmo = actor.GetEquippedWeapon().data.hasInfiniteBackupAmmo ? int.MinValue : actor.GetEquippedWeapon().amount;
		int currentAmmo = actor.GetEquippedWeaponAmmo();

		return (currentAmmo, totalAmmo);
	}

	private void HandlePlayerDeath()
	{
		this.enabled = false;
		OnPlayerDeath.Invoke();
	}

	/// <summary>
	/// Don't need params; just update the health UI.
	/// </summary>
	/// <param name="hitLocation"></param>
	/// <param name="hitDirection"></param>
	private void HandleGetHit(Projectile proj)
	{
		UpdateHealthUI();
	}

	private void HandleHeal()
    {
		GameplayUI.Instance.HealthFlash();
		UpdateHealthUI();
    }

	private void UpdateHealthUI()
    {
		GameplayUI.Instance.SetVignette(1f - ((float) actor.HitPoints) / ((float) actor.MaxHitPoints));
	}
}
