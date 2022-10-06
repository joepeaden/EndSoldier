using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		BodyRotationFinished
	}

	// shows state and if actor is in that state
	public Dictionary<State, bool> state;

	[SerializeField] private ActorData data;
	[SerializeField] private Weapon weapon;
	[SerializeField] private GameObject upperBody;

	private CapsuleCollider mainCollider;
    private Rigidbody rb;
    private float moveForce;
    private int hitPoints;

    private void Awake()
    {
		state = new Dictionary<State, bool>()
		{
			{ State.Walking, false },
			{ State.Sprinting, false },
			{ State.Aiming, false },
			{ State.Crouching, false }
		};

		mainCollider = GetComponentInChildren<CapsuleCollider>();
		rb = GetComponent<Rigidbody>();
		hitPoints = data.hitPoints;
	}

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
				ToggleCrouch();
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
	/// Make the actor crouch.
	/// </summary>
	public void ToggleCrouch()
	{
		if (state[State.Crouching])
		{
			mainCollider.gameObject.layer = (int)IgnoreLayerCollisions.CollisionLayers.Default;
			//canBeHit = true;
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
		else
		{
			// need to add clause to this to check if player is actually in cover, not just crouching.
			// set to InCover layer, ignores collisions with bullets
			mainCollider.gameObject.layer = (int)IgnoreLayerCollisions.CollisionLayers.InCover;

			// is canbehit necessary when the layers ignore collision? Probably not.
			//canBeHit = false;
			transform.localScale = new Vector3(1f, .5f, 1f);
		}

		state[State.Crouching] = !state[State.Crouching];
	}

	/// <summary>
	/// Move laterally in moveVector direction. Move force can be found in the ActorData Scriptable Object.
	/// </summary>
	/// <param name="moveVector">Direction of movement.</param>
    public void Move(Vector3 moveVector)
    {
		if (moveVector != Vector3.zero)
		{
			rb.AddForce(moveVector * moveForce);
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
	protected void Die()
	{
		;
	}
}
