﻿using System.Collections;
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

	// these should be in a SO
	public float controllerAimRotaitonSensitivity;
	public float regularSensitivity;
	public float controllerMaxRotationSensitivity;
	public float controllerRotationSensitivity;
	public float aimDeadZoneAngle;

	private Actor actor;
	private Transform reticle;

	// control stuff
	private PlayerControls controls;
	private Vector2 movementInput;
	private Vector2 rotationInput;
	//Quaternion savedRot;
	private bool attemptingToFire;
	private bool triggerPull;

	private void Awake()
	{
		actor = GetComponent<Actor>();
		actor.team = Actor.ActorTeam.Friendly;

		controls = new PlayerControls();
		// Add these to on destroy please please please please I beg you
		controls.Gameplay.Move.performed += HandleMovementInput; 
		controls.Gameplay.Move.canceled += ZeroMovementInput;
		controls.Gameplay.Sprint.performed += HandleSprintPerformedInput;
		controls.Gameplay.Sprint.canceled += HandleSprintStopInput;
		controls.Gameplay.Rotate.performed += HandleRotationInput;
		controls.Gameplay.Aim.performed += HandleAimBeginInput;
		controls.Gameplay.Aim.canceled += HandleAimEndInput;
		controls.Gameplay.Fire.started += HandleFireStartInput;
		//controls.Gameplay.Fire.performed += HandleFirePerformedInput;
		controls.Gameplay.Fire.canceled += HandleFireStopInput;
		controls.Gameplay.SwitchWeapons.performed += HandleSwitchWeaponsInput;
		controls.Gameplay.Reload.performed += HandleReloadInput;
		controls.Gameplay.UseEquipment.performed += HandleUseEquipmentInput;
		controls.Gameplay.Interact.performed += HandleInteractInput;
	}

	private void Start()
	{
		actor.OnDeath.AddListener(HandlePlayerDeath);
		actor.OnGetHit.AddListener(HandleGetHit);
		actor.OnHeal.AddListener(HandleHeal);
		actor.GetInventory().SetWeaponFromData();

		reticle = GameManager.Instance.GetReticleGO()?.transform;

		//StartCoroutine(SetRotation());
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
			if (attemptingToFire)
			{
				actor.AttemptAttack(triggerPull);
				triggerPull = false;
			}

			//if (Input.GetKeyDown(KeyCode.C))
			//{
			//	actor.ToggleCrouch();
			//}
		}
	}

	private void FixedUpdate()
	{
		if (!GameplayUI.Instance || !GameplayUI.Instance.InMenu())
		{
			// rotation is based on movement when sprinting and rotation input when otherwise. So need this.
			Vector3 rotationInputToUse;
			if (!actor.state[Actor.State.Sprinting])
			{
				//// Movement ////
				// get the move direction
				Vector3 moveDir = new Vector3(movementInput.x, 0f, movementInput.y);
				// adjust it for isometric camera pos
				moveDir = Quaternion.AngleAxis(45, Vector3.up) * moveDir;

				moveDir = Vector3.ClampMagnitude(moveDir, 1f);

				actor.Move(moveDir, false);

				rotationInputToUse = rotationInput;
			}
			else
			{
				// only go forward if sprinting
				actor.Move(transform.forward, false);
				rotationInputToUse = movementInput;
			}

			//// Rotation ////
			if (rotationInput != Vector2.zero)
			{
				// Get the angle of rotation based on the controller input (look, math! I did it!)
				float newRotationYAngle = Mathf.Atan(rotationInputToUse.x / rotationInputToUse.y) * Mathf.Rad2Deg;

				// handle the wierd problem with negative y values (idk why man it works ok?)
				if (rotationInputToUse.y < 0)
				{
					newRotationYAngle -= 180;
				}

				newRotationYAngle += 45f;

				float rotationDelta = Mathf.Abs(newRotationYAngle - transform.rotation.eulerAngles.y);


					// fix if we're going from 360 to 0 or the other way; this is confusing but don't stress it.
				// basically just need to remember that transform.Rotate tatkes a number of degrees to rotate as a param. So going from 359 -> 0  degree rotation should not be -359 degrees, but should be 1 degree. Ya feel me?
				if (rotationDelta >= 180f)
				{
					rotationDelta -= 359f;
				}

				float stratifiedRotation = rotationDelta / controllerMaxRotationSensitivity;
				float adjustedRotationDelta = stratifiedRotation * (actor.state[Actor.State.Aiming] ? controllerAimRotaitonSensitivity : controllerRotationSensitivity);
				float adjustedRotationValue = transform.rotation.eulerAngles.y > newRotationYAngle ? -adjustedRotationDelta : adjustedRotationDelta;

				transform.Rotate(new Vector3(0f, adjustedRotationValue, 0f));
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

	private void HandleSprintPerformedInput(InputAction.CallbackContext cntxt)
	{
		actor.SetState(Actor.State.Sprinting);
		actor.OnActorEndAim.Invoke();
	}

	private void HandleSprintStopInput(InputAction.CallbackContext cntxt)
	{
		actor.SetState(Actor.State.Walking);

		// since rotation is based on the movement input when sprinting, clean it up so we don't pop back to previous rot input value.
		rotationInput = movementInput;
	}

	private void HandleRotationInput(InputAction.CallbackContext cntxt)
	{
		Vector2 newRotationInput = cntxt.ReadValue<Vector2>();

		//To implement deadzone, maybe need to save the last rotation compared to that cchanged it and then see if the difference between those has become big enough. Then change again. mayabe?

		//rotationInput = 
	}

	private void HandleAimBeginInput(InputAction.CallbackContext cntxt)
	{
		if (!actor.state[Actor.State.Sprinting])
		{
			actor.SetState(Actor.State.Aiming);
			actor.OnActorBeginAim.Invoke();
		}
	}

	private void HandleAimEndInput(InputAction.CallbackContext cntxt)
	{
		actor.OnActorEndAim.Invoke();
		actor.SetState(Actor.State.Walking);
	}

	private void HandleFireStartInput(InputAction.CallbackContext cntxt)
	{
		// start input callback happens when start and finish input. Kinda wierd if you ask me.
		if (!attemptingToFire)
		{
			triggerPull = true;
			attemptingToFire = true;
		}
	}

	private void HandleFireStopInput(InputAction.CallbackContext cntxt)
    {
        triggerPull = false;
		attemptingToFire = false;
    }

	private void HandleSwitchWeaponsInput(InputAction.CallbackContext cntxt)
	{
		if (actor.GetInventory().weaponCount > 1)
		{
			bool result = actor.AttemptSwitchWeapons();
			if (result == true)
			{
				OnSwitchWeapons.Invoke(actor.GetEquippedWeapon());
			}
		}
	}

	private void HandleReloadInput(InputAction.CallbackContext cntxt)
	{
		actor.AttemptReload();
	}

	private void HandleUseEquipmentInput(InputAction.CallbackContext cntxt)
	{
		actor.AttemptUseEquipment();
		OnUpdateEquipment.Invoke(actor.GetInventory().GetEquipment());
	}

	private void HandleInteractInput(InputAction.CallbackContext cntxt)
	{
		actor.AttemptInteraction();

		// hmmmmm I haven't figured out how to handle swapping equipment, etc. Or even if I will allow that.
		OnUpdateEquipment.Invoke(actor.GetInventory().GetEquipment());
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
