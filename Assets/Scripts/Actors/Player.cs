using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// Controller for the player, also handles a few player specific things like death.
/// </summary>
public class Player : MonoBehaviour
{
	public UnityEvent<InventoryWeapon> OnSwitchWeapons = new UnityEvent<InventoryWeapon>();
	public UnityEvent<Equipment> OnUpdateEquipment = new UnityEvent<Equipment>();
	public UnityEvent OnPlayerDeath = new UnityEvent();

	private Actor actor;
	private Transform reticle;
	private bool triggerPull;
	private PlayerControls controls;

	private void Awake()
	{
		actor = GetComponent<Actor>();
		actor.team = Actor.ActorTeam.Friendly;

		controls = new PlayerControls();
		// throw away the parameter... hmm. cool.
		controls.Gameplay.Move.performed += x => testMethod();
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

	void testMethod()
    {
		Debug.Log("Up!");
    }

	private void Update()
	{
		if (!GameplayUI.Instance || !GameplayUI.Instance.InMenu())
		{

			if (Input.GetKey(KeyCode.LeftShift))
			{
				actor.SetState(Actor.State.Sprinting);
			}
			else if (Input.GetButton("Fire2"))
			{
				actor.SetState(Actor.State.Aiming);
				actor.OnActorBeginAim.Invoke();
			}
			else
			{
				actor.OnActorEndAim.Invoke();
				actor.SetState(Actor.State.Walking);
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
			// Normalized direction to shoot the projectile
			//Vector2 aimVector = (reticle.position - transform.position).normalized;
			Vector3 retPos = reticle.position;
			//retPos.y = transform.position.y;
			actor.UpdateActorRotation(retPos);

			if (actor.state[Actor.State.Sprinting])
			{
				// only go forward if sprinting
				actor.Move(transform.forward, false);
			}
			else
			{
				Vector3 moveDir = Vector3.zero;
				// Movement inputs
				if (Input.GetKey(KeyCode.W))
				{
					moveDir += Vector3.forward + Vector3.right;
				}
				if (Input.GetKey(KeyCode.S))
				{
					moveDir += -Vector3.forward - Vector3.right;
				}
				if (Input.GetKey(KeyCode.A))
				{
					moveDir += -Vector3.right + Vector3.forward;
				}
				if (Input.GetKey(KeyCode.D))
				{
					moveDir += Vector3.right - Vector3.forward;
				}

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
