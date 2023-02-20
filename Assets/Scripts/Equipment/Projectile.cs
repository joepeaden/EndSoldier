using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // move this stuff into a Scriptable.

    [SerializeField]
	protected float velocity;
    [SerializeField]
    public float impact;
    [SerializeField]
    protected float range;
    [SerializeField]
    protected int damage;
    [SerializeField]
    protected bool isExplosive;
    [SerializeField]
    protected Actor owningActor;

    private Vector3 lastPoint;

    private void Awake()
    {
        lastPoint = transform.position;
    }

    private void FixedUpdate()
    {
        if (velocity != 0)

            GetComponent<Rigidbody>().velocity = velocity * transform.forward;
        else
            Debug.LogWarning("Projectile velocity for " + gameObject.name + " is not set");

        Vector3 movementSinceLastFrame = (transform.position - lastPoint);
        Physics.Raycast(lastPoint, movementSinceLastFrame.normalized, out RaycastHit hitInfo, movementSinceLastFrame.magnitude);
        if (hitInfo.collider != null)
        {
            OnTriggerEnter(hitInfo.collider);
        }
        lastPoint = transform.position;
    }

    void OnTriggerEnter (Collider other)
    {
    	Actor actor = other.gameObject.GetComponentInParent<Actor>();

        // please don't kill yo self or teammates
        if (actor != null && actor.team == owningActor.team)
            return;

        // don't destroy if hit actor's collider (only do so on hit box)
        bool shouldDestroy = other.CompareTag("HitBox") || (!other.isTrigger && actor == null);

        // only hit an actor if it's the actor's hit box
        if (actor != null && other.gameObject.GetComponent<HitBox>())
	    {
            // may not always destroy if hit actor, i.e. if actor is crouching and it "missed"
            shouldDestroy = actor.GetHit(damage);

            if(isExplosive)
            {
                // implement method per projectile types
                CreateExplosion();
            }
        }

        // only destroy projectiles if they hit something solid
        if (shouldDestroy)
        {
            Destroy(gameObject);
        }
    }

    public void SetOwningActor(Actor actor)
    {
        // set the actor who fired this projectile
        owningActor = actor;
    }

    protected void CreateExplosion()
    {
        Debug.Log("Boom");
    }
}
