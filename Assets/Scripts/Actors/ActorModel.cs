using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorModel : MonoBehaviour
{
    [SerializeField]
    private Actor actor;
    [SerializeField]
    private GameObject ragdoll;
    [SerializeField]
    private Animator ragAnim;

    private List<Rigidbody> ragRigidBodies;
    private List<Collider> ragColliders;

    void Start()
    {
        //actor.OnCrouch.AddListener(HandleCrouch);
        //actor.OnStand.AddListener(HandleStand);
        actor.OnDeath.AddListener(SwapToRagdoll);

        ragRigidBodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        ragColliders = new List<Collider>(GetComponentsInChildren<Collider>());
        
        foreach (Collider c in ragColliders)
        {
            c.enabled = false;
        }
    }

    public void UpdateVelocityBasedAnimations(Vector3 velocity)
    {
        float vert = Vector3.Dot(velocity, transform.forward);
        float horiz = Vector3.Dot(velocity, transform.right);

        ragAnim.SetFloat("Velocity", velocity.magnitude);
        ragAnim.SetFloat("VerticalAxis", vert);
        ragAnim.SetFloat("HorizontalAxis", horiz);
    }

    private void SwapToRagdoll()
    {
        //    ragdoll.SetActive(true);
        //    model.SetActive(false);

        foreach (Rigidbody rb in ragRigidBodies)
        {
            rb.isKinematic = false;
        }

        foreach (Collider c in ragColliders)
        {
            c.enabled = true;
        }

        ragAnim.enabled = false;
    }

    private void HandleCrouch()
    {
        //transform.localScale = new Vector3(1f, .5f, 1f);
        //transform.localPosition = new Vector3(0f, 0f, 0f);
    }

    private void HandleStand()
    {
        //transform.localScale = originalDimensions;
        //transform.localPosition = new Vector3(0f, 0.5f, 0f);
    }

    private void OnDestroy()
    {
        actor.OnDeath.RemoveListener(SwapToRagdoll);
    }
}
