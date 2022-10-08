using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Author: Joseph Peaden

/// <summary>
/// Main player script.
/// </summary>
public class Player : MonoBehaviour
{
	public static UnityAction OnPlayerBeginAim;
	public static UnityAction OnPlayerEndAim;

	private Actor actor;
	private Transform reticle;

	private void Awake()
    {
		actor = GetComponent<Actor>();
    }

    private void Start()
    {
		reticle = GameManager.Instance.GetReticleGO().transform;
    }

    private void Update()
	{
		if (Input.GetKey(KeyCode.LeftShift))
		{
			if (!actor.state[Actor.State.Sprinting])
			{
				actor.SetState(Actor.State.Sprinting);
			}
		}
		else if (Input.GetButton("Fire2"))
		{
			if (!actor.state[Actor.State.Aiming])
			{
				actor.SetState(Actor.State.Aiming);
				OnPlayerBeginAim.Invoke();
			}
		}
		else if (!actor.state[Actor.State.Walking])
		{
			if (actor.state[Actor.State.Aiming])
			{
				OnPlayerEndAim.Invoke();
			}

			actor.SetState(Actor.State.Walking);
		}

		if (Input.GetButton("Fire1") && !actor.state[Actor.State.Sprinting])

		{
			actor.AttemptAttack();
        }

		if (Input.GetKeyDown(KeyCode.R))
		{
			actor.AttemptReload();
        }

		if (Input.GetKeyDown(KeyCode.C))
        {
			actor.ToggleCrouch();
        }

		if (Input.GetKeyDown(KeyCode.Space))
        {
			actor.AttemptDuckInCover();
        }
	}

    private void FixedUpdate()
    {
		// normalized direction to shoot the projectile
		Vector2 aimVector = (reticle.position - transform.position).normalized;
		actor.UpdateAim(aimVector);

		if (Input.GetKey(KeyCode.A))
		{
			actor.Move(-Vector3.right);
		}
		if (Input.GetKey(KeyCode.D))
		{
			actor.Move(Vector3.right);
		}
	}
}
