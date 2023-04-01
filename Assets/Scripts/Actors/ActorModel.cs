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
    private List<GameObject> bodyParts = new List<GameObject>();

    //private bool isHighlighted;

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

        // can just assign these in the inspector if optimization is needed.
        SkinnedMeshRenderer[] smrs = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer smr in smrs)
        {
            if (smr.gameObject.activeInHierarchy)
            {
                bodyParts.Add(smr.gameObject);
            }
        }

        //StartCoroutine(CheckOutline());
    }

    // not using it, but just in case, who knows if i'll need it later
    // ya know what i mean?
    // work was tough this week.
    //private IEnumerator CheckOutline()
    //{
    //    while (actor.IsAlive)
    //    {
    //        Ray r = new Ray(Camera.main.transform.position, transform.position - Camera.main.transform.position);
    //        if (Physics.Raycast(ray: r, out RaycastHit hitInfo, maxDistance: float.MaxValue, layerMask: ~LayerMask.GetMask(LayerNames.CollisionLayers.MouseOnly.ToString())))
    //        {
    //            // should highlight
    //            if (hitInfo.transform.GetComponent<Actor>() == null && hitInfo.transform.GetComponentInParent<Actor>() == null)
    //            {
    //                if (!isHighlighted)
    //                {
    //                    gameObject.layer = actor.IsPlayer ? (int)LayerNames.CollisionLayers.PlayerOutline : (int)LayerNames.CollisionLayers.EnemyOutline;

    //                    // this could be waaay optimized.
    //                    //Transform[] transforms = GetComponentsInChildren<Transform>();
    //                    foreach (GameObject g in bodyParts)
    //                    {
    //                        g.layer = actor.IsPlayer ? (int)LayerNames.CollisionLayers.PlayerOutline : (int)LayerNames.CollisionLayers.EnemyOutline;
    //                    }

    //                    isHighlighted = true;
    //                }
    //            }
    //            // should unhighlight
    //            else if (isHighlighted)
    //            {
    //                gameObject.layer = (int)LayerNames.CollisionLayers.IgnoreActors;

    //                foreach (GameObject g in bodyParts)
    //                {
    //                    g.layer = (int)LayerNames.CollisionLayers.IgnoreActors;
    //                }

    //                isHighlighted = false;
    //            }
    //        }

    //        yield return new WaitForSeconds(.1f);
    //    }
    //}

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

        foreach (GameObject g in bodyParts)
        {
            g.layer = (int) LayerNames.CollisionLayers.IgnoreActors;
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
