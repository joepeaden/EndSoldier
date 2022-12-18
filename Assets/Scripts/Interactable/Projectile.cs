using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
	protected float force;
    [SerializeField]
    public float impact;
    [SerializeField]
    protected float range;
    [SerializeField]
    protected int damage;
    [SerializeField]
    protected bool isExplosive;

    void Update()
    {
        if (force != 0)
            GetComponent<Rigidbody>().AddForce(force * transform.forward);
        else
            Debug.LogWarning("Projectile force for " + gameObject.name + " is not set");

        // record distance travelled, delete if outside range
        // or just have boundary collidr?
    }

    void OnTriggerEnter (Collider other)
    {
    	Actor actor = other.gameObject.GetComponent<Actor>();

        bool shouldDestroy = !other.isTrigger;
        if (actor != null)
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

    protected void CreateExplosion()
    {
        Debug.Log("Boom");
    }
}
