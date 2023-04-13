﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

[RequireComponent(typeof(Inventory))]

/// <summary>
/// Superclass for any actor (player, enemy, etc.)
/// Actors are the bodies of enemy and player (as opposed to the Controller i.e. Player.cs, Enemy.cs being the brain)
/// </summary>
public class Actor : MonoBehaviour
{
	public UnityAction OnActorBeginAim;
	public UnityAction OnActorEndAim;
	public UnityEvent OnDeath = new UnityEvent();
	public UnityEvent OnGetHit = new UnityEvent();
	public UnityEvent OnCrouch = new UnityEvent();
	public UnityEvent OnStand = new UnityEvent();
	public UnityEvent<Vector3> EmitVelocityInfo = new UnityEvent<Vector3>();

	public enum State
	{
		Walking,
		Sprinting,
		Aiming,
		Crouching,
		// true if the upperbody is facing the desired aim direction
		BodyRotationFinished,
		InCover
	}

	public enum ActorTeam
    {
		Enemy,
		Friendly
    }

	// shows state and if actor is in that state
	public Dictionary<State, bool> state;
	public bool IsAlive { get; private set; } = true;
	public bool IsPlayer;
	public int HitPoints { get; private set; }
	public int MaxHitPoints { get { return data.hitPoints; } }

	public ActorData data;
	[SerializeField] private MeshRenderer modelRenderer;
	[SerializeField] private Transform TargetSpot;
	[SerializeField] private ActorModel actorModel;
	[SerializeField] private GameObject bloodPoofEffect;

	// temporary to visually show cover status. Remove once we have models, animations etc.
	//[SerializeField] private Material originalMaterial;
	//[SerializeField] private Material coverMaterial;

	[Header("Debug Options")]
	[SerializeField] private bool isInvincible;

    #region Components
    private ActorInteractSensor interactSensor;
	private CapsuleCollider mainCollider;
    private Rigidbody rigidBody;
	private NavMeshAgent navAgent;
	private AudioSource audioSource;
	private Inventory inventory;
	#endregion

	private float moveForce;
	// position before actor enters cover (for returning to correct position)
	//private Vector3 posBeforeCover;
	// is the EnterExitCover coroutine running?
	//private bool coverCoroutineRunning;
	// original dimensions of actor object
	//private Vector3 originalModelDimensions;

	public Vector3 lookTarget;
	/// <summary>
    /// Not sure if it's really the actor's target, but a guess.
    /// </summary>
	public Transform target;
	public ActorTeam team;

	private bool isUsingNavAgent;
	private List<AudioClip> deathSounds;
	private List<AudioClip> woundSounds;
	private Vector3 lastHitLocation;
	private bool movingToCover;
	private Cover targetCover;

	private void Awake()
    {
		state = new Dictionary<State, bool>()
		{
			{ State.Walking, false },
			{ State.Sprinting, false },
			{ State.Aiming, false },
			{ State.Crouching, false },
			{ State.BodyRotationFinished, true },
			{ State.InCover, false }
		};

		mainCollider = GetComponent<CapsuleCollider>();
		interactSensor = GetComponentInChildren<ActorInteractSensor>();
		rigidBody = GetComponent<Rigidbody>();
		navAgent = GetComponent<NavMeshAgent>();
		audioSource = GetComponent<AudioSource>();
		inventory = GetComponent<Inventory>();
		HitPoints = data.hitPoints;

		deathSounds = new List<AudioClip>()
		{
			data.deathSound1,
			data.deathSound2,
			data.deathSound3
		};

		woundSounds = new List<AudioClip>()
		{
			data.woundSound1,
			data.woundSound2,
			data.woundSound3,
			data.woundSound4,
			data.woundSound5,
			data.woundSound6,
		};
	}

    private void OnDestroy()
    {
		OnDeath.RemoveAllListeners();
		OnGetHit.RemoveAllListeners();
		EmitVelocityInfo.RemoveAllListeners();
		OnCrouch.RemoveAllListeners();
	    OnStand.RemoveAllListeners();
	}

	private void LateUpdate()
	{
		EmitVelocityInfo.Invoke(isUsingNavAgent ? navAgent.velocity : rigidBody.velocity);
	}

    private void OnCollisionEnter(Collision collision)
    {
		Cover collisionCover = collision.gameObject.GetComponent<Cover>();

		if (collisionCover && collisionCover == targetCover)
        {
			movingToCover = false;
        }
    }

	public void SetAgentSpeed(float speed)
    {
		navAgent.speed = speed;
    }

    public void SetVisibility(bool visible)
    {
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(visible);
		}
	}

    // Either this method needs to be done away with or it needs to be only internal... I don't think
    // that actors should be calling this method. They should just call a method like ToggleCrouch() instead of 
    // SetState(Crouch). Figure out how to handle states. Don't want the impression that this is the only place 
    // that states are modfified if it isn't

    /// <summary>
    /// Set a state of the actor.
    /// </summary>
    /// <param name="stateToModify">The state to activate.</param>
    public void SetState(State stateToModify)
	{
		switch (stateToModify)
		{
			case State.Sprinting:
				state[State.Sprinting] = true;
				state[State.Walking] = false;
				state[State.Aiming] = false;
				moveForce = data.sprintMoveForce;
				break;
			case State.Walking:
				state[State.Walking] = true;
				state[State.Sprinting] = false;
				state[State.Aiming] = false;
				moveForce = data.fastWalkMoveForce;
				break;
			case State.Aiming:
				state[State.Aiming] = true;
				state[State.Sprinting] = false;
				state[State.Walking] = false;
				moveForce = data.slowWalkMoveForce;
				break;
			case State.Crouching:
				state[State.Crouching] = true;
				break;
			default:
				break;
		}
	}

	public Transform GetShootAtMeTransform()
    {
		return TargetSpot;
    }

	/// <summary>
	/// Rotate the actor to look at lookTarget.
	/// </summary>
	/// <param name="lookTarget">The target to look at.</param>
	public void UpdateActorRotation(Vector3 newLookTarget)
	{
		//Vector3 rotation = Quaternion.LookRotation(newLookTarget).eulerAngles;
		////rotation.y = 0f;

		//transform.rotation = Quaternion.Euler(rotation);

        newLookTarget.y = transform.position.y;
        lookTarget = newLookTarget;
        transform.LookAt(lookTarget);
	}

	public bool AttemptSwitchWeapons()
    {
		if (inventory != null)
        {
			if (inventory.AttemptSwitchWeapons())
            {
				return true;
            }
        }

		return false;
    }

	/// <summary>
	/// Attempt an attack with equipped weapon.
	/// </summary>
	/// <returns>Whether the attack was made or not.</returns>
    /// <param name="triggerPull">Is this attack the result of an initial trigger pull, as opposed to holding down the trigger?</param>
	public bool AttemptAttack(bool triggerPull)
    {
		if (inventory != null)
		{
			return inventory.AttemptUseWeapon(triggerPull);
		}

		return false;
    }

	/// <summary>
	/// Attempt to reload equipped weapon.
	/// </summary>
	/// <
	/// <returns>Whether the reload was successful or not.</returns>
	public bool AttemptReload()
    {
		// need to add feedback sound to indicate they're out of ammo

		if (inventory != null)
		{
			inventory.AttemptStartReload();
		}

		return true;
	}

	/// <summary>
	/// Get actor's equipped weapon's ammo count.
	/// </summary>
	/// <returns>Amount of ammo in weapon.</returns>
	public int GetEquippedWeaponAmmo()
	{
		if (inventory != null)
		{
			return inventory.GetEquippedWeaponAmmo();
		}

		return 0;
	}

	public InventoryWeapon GetEquippedWeapon()
    {
		if (inventory != null)
        {
			return inventory.GetEquippedWeapon();
        }

		return null;
    }

	/// <summary>
	/// Toggle the actor crouch.
	/// </summary>
	public void ToggleCrouch()
	{
		if (state[State.Crouching])
		{
			SetCrouch(false);
		}
		else
		{
			SetCrouch(true);
		}
	}

	/// <summary>
	/// Make the actor crouch.
	/// </summary>
	public void SetCrouch(bool shouldCrouch)
	{
		if (shouldCrouch)
		{
			OnCrouch.Invoke();
		}
		else
		{
			OnStand.Invoke();
		}

		state[State.Crouching] = shouldCrouch;
	}


	/// <summary>
	/// Adds a listener to the CoverSensor's OnCoverNearby event.
	/// </summary>
	/// <param name="listener">The method to trigger when nearby cover is detected</param>
	public void AddCoverListener(UnityAction listener)
    {
		interactSensor.OnInteractableNearby.AddListener(listener);
	}

	public bool AttemptInteraction()
    {
		Interactable interactable = interactSensor.GetInteractable();
		if (interactable)
		{
			interactable.Interact(this);
        }

		return false;
    }

	/// <summary>
	/// Attempt to move the actor to the actorTargetPosition of a cover object, as well as change the collisions layer and visuals for the actor.
	/// </summary>
	/// <returns>Whether or not the attempt was successful.</returns>
	private bool AttemptDuckInCover(Cover cover)
    {
		//if (!cover)
  //      {
		//	return false;
  //      }
		
		//if (cover && !state[State.InCover] && !coverCoroutineRunning)
		//{
		//	modelRenderer.material = coverMaterial;

		//	// set to InCover layer, ignores collisions with bullets
		//	mainCollider.gameObject.layer = (int)IgnoreLayerCollisions.CollisionLayers.InCover;

		//	StartCoroutine(EnterOrExitCover(true));

		//	posBeforeCover = transform.position;

		//	return true;
		//}

		return false;
    }

	/// <summary>
	/// Attempt to exit a cover object.
	/// </summary>
	/// <returns>Whether or not the attempt was successful.</returns>
	public bool AttemptExitCover()
	{
		//if (!interactSensor.GetInteractable() || !state[State.InCover])
  //      {
		//	Debug.LogWarning("AttemptExitCover was called, but no cover or actor not in cover state");
		//	return false;
  //      }

		//if (!coverCoroutineRunning)
		//{
		//	modelRenderer.material = originalMaterial;

		//	mainCollider.gameObject.layer = (int)IgnoreLayerCollisions.CollisionLayers.Actors;

		//	StartCoroutine(EnterOrExitCover(false));
			
		//	return true;
		//}

		return false;
	}

	/// <summary>
	/// Enter or exit a cover object.
	/// </summary>
	/// <param name="enteringCover">True if the actor is entering cover.</param>
	/// <returns></returns>
	private IEnumerator EnterOrExitCover(bool enteringCover)
	{
		//Cover cover = interactSensor.GetInteractable().GetComponent<Cover>();
		//if (!cover)
		//{
		//	Debug.LogWarning("StartDuckInCover is called but no cover is set in the sensor.");
		//	yield return null;
		//}
		//else
		//{
		//	coverCoroutineRunning = true;

		//	if (cover.coverType == Cover.CoverType.Floor)
		//	{
		//		ToggleCrouch();
		//	}

		//	rigidBody.velocity = Vector3.zero;

		//	//Collider c = interactSensor.GetComponent<Collider>();
		//	Vector3 closestPoint = interactSensor.GetInteractableCollider().ClosestPointOnBounds(interactSensor.transform.position);


		//	Vector3 targetPos = enteringCover ? closestPoint : posBeforeCover;
		//	targetPos.y = transform.position.y;

		//	targetCover = cover;
		//	movingToCover = true;
		//	do
		//	{
		//		var step = data.moveToCoverSpeed * Time.deltaTime;
		//		transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

				yield return null;
		//	} while (enteringCover ? movingToCover : transform.position != targetPos);	

		//	state[State.InCover] = enteringCover;

		//	coverCoroutineRunning = false;
		//}
	}

	private bool AttemptVaultOverCover()
    {
		//Cover cover = interactSensor.GetInteractable().GetComponent<Cover>();
		//if (cover && cover.coverType == Cover.CoverType.Floor)
  //      {
		//	// when implementing animations, will have a vault over animation here. for now, just move through.
		//	cover.GetComponent<Collider>().enabled = false;

		//	cover.GetActorFlipPosition(transform.position);

		//	return true;
  //      }

		return false;
    }

	public bool AttemptUseEquipment()
    {
		if (inventory != null)
		{
			return inventory.AttemptUseEquipment();
		}

		return false;
	}

	public void PickupLoot(Loot loot)
    {
		inventory.AttemptAddItem(loot.item);
    }

	/// <summary>
	/// Move laterally in moveVector direction. Move force can be found in the ActorData Scriptable Object.
	/// </summary>
	/// <param name="useNavMesh">Should this actor use NavMesh? Affects how moveVector is interpeted.</param>
	/// <param name="moveVector">If not useNavMesh, direction of movement. If useNavMesh, the destination of the agent.</param>
	public void Move(Vector3 moveVector, bool useNavMesh = true)
    {
		isUsingNavAgent = useNavMesh;

		if (moveVector != Vector3.zero)
		{
			if (useNavMesh)
			{
				if(navAgent == null)
                {
					Debug.LogWarning("No NavMeshAgent attatched to actor " + gameObject.name + ", but attempted to use it.");
                }
				else if (navAgent.destination != moveVector)
				{
					navAgent.destination = moveVector;
				}
			}
			else
			{
				rigidBody.AddForce(moveVector * moveForce);
			}

			// if actor tries to move, exit cover
			if (state[State.InCover])
            {
                AttemptExitCover();
            }
        }
		else if (useNavMesh)
        {
			navAgent.destination = transform.position;
        }
	}

    public void StopMoving()
    {
		Move(Vector3.zero);
    }

    public Inventory GetInventory()
    {
        return inventory;
    }

    /// <summary>
    /// Take a specified amount of damage.
    /// </summary>
    /// <param name="damage">Damage to deal to this actor.</param>
    /// <returns>If the projectile should </returns>
    public bool GetHit(int damage, Vector3 hitLocation)
	{
		lastHitLocation = hitLocation;

		bool gotHit = true;
		if (!IsAlive || isInvincible)
        {
			return gotHit;
        }

		if (state[State.Crouching])
		{
			float dogeRoll = Random.Range(0f, 1f);

			Debug.Log("Chance: " + data.crouchDogeChance + "Rolled: " + dogeRoll);

			gotHit = dogeRoll > data.crouchDogeChance;
		}

		if (gotHit)
		{
			HitPoints -= damage;

			// don't always play the sound.
			int index = Random.Range(0, woundSounds.Count * 3);
			if (index < woundSounds.Count)
			{
				PlaySound(woundSounds[index]);
			}


			OnGetHit.Invoke();

			if (HitPoints <= 0)
			{
				Die();
			}
		}

		return gotHit;
	}

	/// <summary>
    /// Play a sound using the Actor's audio source.
    /// </summary>
    /// <param name="clip"></param>
	public void PlaySound(AudioClip clip)
    {
		audioSource.clip = clip;
		audioSource.Play();
	}

	/// <summary>
	/// Kill this actor.
	/// </summary>
	private void Die()
	{
		IsAlive = false;

		// disable components
		navAgent.enabled = false;
		mainCollider.enabled = false;

		int index = Random.Range(0, deathSounds.Count * 3);
		if (index < deathSounds.Count)
		{
			PlaySound(deathSounds[index]);
		}

		lastHitLocation = new Vector3(lastHitLocation.x, lastHitLocation.y + Random.Range(0f, .5f), lastHitLocation.z);
		Instantiate(bloodPoofEffect, lastHitLocation, Quaternion.identity);

		// have actor handle it's own inevitable destruction. It's ok buddy.
		OnDeath.Invoke();
	}
}
