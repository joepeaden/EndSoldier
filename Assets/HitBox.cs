using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Collider for registering hits on this actor.
/// </summary>
public class HitBox : MonoBehaviour
{
    private Actor actor;
    private Vector3 originalDimensions;

    void Start()
    {
        originalDimensions = transform.localScale;
        actor = GetComponentInParent<Actor>();
        actor.OnCrouch.AddListener(HandleCrouch);
        actor.OnStand.AddListener(HandleStand);
    }

    private void HandleCrouch()
    {
        transform.localScale = new Vector3(1f, .5f, 1f);
        transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    private void HandleStand()
    {
	    transform.localScale = originalDimensions;
	    transform.localPosition = new Vector3(0f, 0.5f, 0f);
	}
}
