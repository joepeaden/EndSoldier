using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Author: Joseph Peaden

/// <summary>
/// Superclass for any actor (player, enemy, etc.)
/// </summary>
public class Actor : MonoBehaviour
{
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

	// shows state and if actor is in that state
	public Dictionary<State, bool> state;
	
	[SerializeField] private ActorData data;
	[SerializeField] private Weapon weapon;
	[SerializeField] private GameObject modelGO;
	[SerializeField] private GameObject upperBody;

	// temporary to visually show cover status. Remove once we have models, animations etc.
	[SerializeField] private Material originalMaterial;
	[SerializeField] private Material coverMaterial;

	private ActorCoverSensor coverSensor;
	private CapsuleCollider mainCollider;
    private Rigidbody rigidBody;

	// values
	private float moveForce;
    private int hitPoints;
	// position before actor enters cover (for returning to correct position)
	private Vector3 posBeforeCover;
	// is the EnterExitCover coroutine running?
	private bool coverCoroutineRunning;
	// original dimensions of actor object
	private Vector3 originalDimensions;
	// for flipping the coverSensor's position when going different directions
	private int previousXMoveDirection;

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

		mainCollider = GetComponentInChildren<CapsuleCollider>();
		coverSensor = GetComponentInChildren<ActorCoverSensor>();

		rigidBody = GetComponent<Rigidbody>();
		hitPoints = data.hitPoints;

		originalDimensions = transform.localScale;
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
	
	/// <summary>
	/// Rotate the actor's aim to point in aimVector direction.
	/// </summary>
	/// <param name="aimVector">The direction to aim in.</param>
	public void UpdateAim(Vector2 aimVector)
	{
		// no idea what this math is.
		float angle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		// if we cross from 360 - 0 or the other way around, handle it
		bool crossedZeroDown = rotation.eulerAngles.z > 180 && upperBody.transform.rotation.eulerAngles.z < 90;
		bool crossedZeroUp = rotation.eulerAngles.z < 90 && upperBody.transform.rotation.eulerAngles.z > 180;

		if (Mathf.Abs(rotation.eulerAngles.z - upperBody.transform.rotation.eulerAngles.z) < 10)
		{
			upperBody.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			upperBody.GetComponent<Rigidbody>().MoveRotation(rotation);
			state[State.BodyRotationFinished] = true;
			return;
		}
		else if (!crossedZeroDown && rotation.eulerAngles.z > upperBody.transform.rotation.eulerAngles.z || crossedZeroUp)
		{
			upperBody.GetComponent<Rigidbody>().AddTorque(Vector3.forward * data.rotationTorque);
		}
		else
		{
			upperBody.GetComponent<Rigidbody>().AddTorque(Vector3.forward * -data.rotationTorque);
		}

		state[State.BodyRotationFinished] = false;
	}

	/// <summary>
	/// Attempt an attack with equipped weapon.
	/// </summary>
	/// <returns>Whether the attack was made or not.</returns>
	public bool AttemptAttack()
    {
		if (weapon && weapon.HasAmmo())
        {
			weapon.InitiateAttack(data.recoilControl);
			
			return true;
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

		weapon.StartReload();

		return true;
	}

	/// <summary>
	/// Get actor's equipped weapon's ammo count.
	/// </summary>
	/// <returns>Amount of ammo in weapon.</returns>
	public int GetEquippedWeaponAmmo()
    {
		return weapon.GetAmmo();
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
			transform.localScale = new Vector3(1f, .5f, 1f);
		}
		else
		{
			transform.localScale = originalDimensions;
		}

		state[State.Crouching] = shouldCrouch;
	}


	/// <summary>
	/// Adds a listener to the CoverSensor's OnCoverNearby event.
	/// </summary>
	/// <param name="listener">The method to trigger when nearby cover is detected</param>
	public void AddCoverListener(UnityAction listener)
    {
		coverSensor.OnCoverNearby.AddListener(listener);
	}

	/// <summary>
	/// Attempt to move the actor to the actorTargetPosition of a cover object, as well as change the collisions layer and visuals for the actor.
	/// </summary>
	/// <returns>Whether or not the attempt was successful.</returns>
	public bool AttemptDuckInCover()
    {
		Cover cover = coverSensor.GetCover();
		
		if (cover && !state[State.InCover] && !coverCoroutineRunning)
		{
			modelGO.GetComponent<MeshRenderer>().material = coverMaterial;

			// set to InCover layer, ignores collisions with bullets
			mainCollider.gameObject.layer = (int)IgnoreLayerCollisions.CollisionLayers.InCover;

			if (cover.coverType == Cover.CoverType.Floor)
			{
				SetCrouch(true);
			}

			StartCoroutine(EnterOrExitCover(true));

			posBeforeCover = transform.position;
			// ensure that the original pos goes back to the zero z position
			posBeforeCover.z = 0f;

			return true;
		}

		return false;
    }

	/// <summary>
	/// Attempt to exit a cover object.
	/// </summary>
	/// <returns>Whether or not the attempt was successful.</returns>
	public bool AttemptExitCover()
	{
		if (!coverSensor.GetCover() || !state[State.InCover])
        {
			Debug.LogWarning("AttemptExitCover was called, but no cover or actor not in cover state");
			return false;
        }

		if (!coverCoroutineRunning)
		{
			modelGO.GetComponent<MeshRenderer>().material = originalMaterial;

			mainCollider.gameObject.layer = (int)IgnoreLayerCollisions.CollisionLayers.Default;

			StartCoroutine(EnterOrExitCover(false));
			
			return true;
		}

		return false;
	}

	/// <summary>
	/// Enter or exit a cover object.
	/// </summary>
	/// <param name="enteringCover">True if the actor is entering cover.</param>
	/// <returns></returns>
	private IEnumerator EnterOrExitCover(bool enteringCover)
	{
		if (!coverSensor.GetCover())
		{
			Debug.LogWarning("StartDuckInCover is called but no cover is set in the sensor.");
			yield return null;
		}
		else
		{
			coverCoroutineRunning = true;

			rigidBody.velocity = Vector3.zero;

			Vector3 targetPos = enteringCover ? coverSensor.GetCover().GetActorCoverPosition(transform.position) : posBeforeCover;
			targetPos.y = transform.position.y;

			do
			{
				var step = data.moveToCoverSpeed * Time.deltaTime;
				transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

				yield return null;
			} while (transform.position != targetPos);	

			state[State.InCover] = enteringCover;

			coverCoroutineRunning = false;
		}
	}

	private bool AttemptVaultOverCover()
    {
		Cover cover = coverSensor.GetCover();
		if (cover && cover.coverType == Cover.CoverType.Floor)
        {
			// when implementing animations, will have a vault over animation here. for now, just move through.
			cover.GetComponent<Collider>().enabled = false;

			cover.GetActorFlipPosition(transform.position);
        }

		return false;
    }

	/// <summary>
	/// Move laterally in moveVector direction. Move force can be found in the ActorData Scriptable Object.
	/// </summary>
	/// <param name="moveVector">Direction of movement.</param>
	public void Move(Vector3 moveVector)
    {
		
		// if we're moving right, move cover sensor to face that direction. Otherwise, the opposite.
		int currentXMoveDirection = rigidBody.velocity.x > 0 ? 1 : -1;
		if (currentXMoveDirection != previousXMoveDirection)
		{
			Vector3 newCoverSensorPos = coverSensor.transform.localPosition;
			newCoverSensorPos.x = -1 * newCoverSensorPos.x;
			coverSensor.transform.localPosition = newCoverSensorPos;
			previousXMoveDirection = currentXMoveDirection;
		}

		if (moveVector != Vector3.zero)
		{
			rigidBody.AddForce(moveVector * moveForce);

			// if actor tries to move, exit cover
			if (state[State.InCover])
            {
				AttemptExitCover();
            }
		}
	}

	/// <summary>
	/// Take a specified amount of damage.
	/// </summary>
	/// <param name="damage">Damage to deal to this actor.</param>
	public void GetHit(int damage)
	{
		hitPoints -= damage;

		if (hitPoints <= 0)
			Die();
	}

	/// <summary>
	/// Kill this actor.
	/// </summary>
	private void Die()
	{
		;
	}
}
