using RootMotion.FinalIK;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// Author: Joseph Peaden

/// <summary>
/// Main player script.
/// </summary>
public class Player : MonoBehaviour
{
	private Actor actor;
	private Transform reticle;
	private bool triggerPull;

	[SerializeField]
	private AimIK aimIK;

	private void Awake()
    {
		actor = GetComponent<Actor>();
    }

    private void Start()
	{
		actor.OnDeath.AddListener(HandlePlayerDeath);
		actor.OnGetHit.AddListener(HandleGetHit);

		reticle = GameManager.Instance.GetReticleGO()?.transform;

		aimIK.solver.target = reticle;
    }

    private void Update()
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
        }
	}

    private void FixedUpdate()
    {
		// Normalized direction to shoot the projectile
		//Vector2 aimVector = (reticle.position - transform.position).normalized;
		Vector3 retPos = reticle.position;
		retPos.y = transform.position.y;
		actor.UpdateActorRotation(retPos);

		var moveVector = Vector3.zero;

		// Movement inputs
		if (Input.GetKey(KeyCode.W))
		{
			moveVector = Vector3.forward + Vector3.right;
		}
		if (Input.GetKey(KeyCode.S))
		{
			moveVector = - Vector3.forward - Vector3.right;
		}
		if (Input.GetKey(KeyCode.A))
		{
			moveVector = -Vector3.right + Vector3.forward;
		}
		if (Input.GetKey(KeyCode.D))
		{
			moveVector = Vector3.right - Vector3.forward;
		}

		actor.Move(moveVector, false);
		actor.UpdateAnimator(moveVector);
	}

    private void OnDestroy()
    {
		actor.OnDeath.RemoveListener(HandlePlayerDeath);
    }

    private void HandlePlayerDeath()
	{
		// for now just make him look ded.
		Quaternion q = new Quaternion();
		q.eulerAngles = new Vector3(0, 0, 90);
		transform.rotation = q;

		this.enabled = false;

		SceneLoader.Instance.LoadScene(SceneLoader.SceneList.FailMenu, true);
	}

	private void HandleGetHit()
    {
		CameraManager.Instance.SetVignette(1f - ((float) actor.HitPoints) / ((float) actor.MaxHitPoints));
    }
}
