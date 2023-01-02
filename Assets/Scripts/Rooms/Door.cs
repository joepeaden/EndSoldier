using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interactable doors.
/// </summary>
public class Door : Interactable
{
    public float openForce;

    private bool isOpening;
    private Rigidbody rb;
    private Collider col;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public override void Interact()
    {
        rb.freezeRotation = false;
        // OPTIMIZE: Could replce this with animation or just not have animation and snap to position. 
        rb.AddForce(isOpening ? openForce * -transform.right : openForce * transform.right);

        isOpening = !isOpening;

        StartCoroutine(ToggleCollisionAfterSwing());
    }

    /// <summary>
    /// Toggles the collision layer for door so it doesn't collide w/ player when opening. Also stops the door from bouncing.
    /// </summary>
    /// <returns></returns>
    public IEnumerator ToggleCollisionAfterSwing() {
        HingeJoint hinge = GetComponent<HingeJoint>();
        col.gameObject.layer = (int)IgnoreLayerCollisions.CollisionLayers.DoorsWhenOpen;

        // if we're opening or closing and haven't reached the stop point of the hinge
        while ((isOpening && hinge.angle > hinge.limits.min - 1f) || (!isOpening && hinge.angle < hinge.limits.max - 1f))
        {
            yield return null;
        }

        rb.freezeRotation = true;
        rb.velocity = Vector3.zero;

        col.gameObject.layer = (int)IgnoreLayerCollisions.CollisionLayers.SightBlocking;
    }

}
