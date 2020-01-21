using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
	protected float speed;
    protected float impact;
    protected float range;
    protected int damage;
    protected bool is_explosive;
 
    void Start()
    {
        Instantiate();
    }

    void Update()
    {
        if(speed != 0)
            transform.Translate(Vector3.right * speed);
        else
            Debug.LogWarning("Projectile speed for " + gameObject.name + " is not set");

        // record distance travelled, delete if outside range+ 
    }

    void OnTriggerEnter2D(Collider2D other)
    {
    	Actor actor = other.GetComponent<Actor>();
    	if(actor != null)
	    {
            actor.GetHit(damage, impact);
            
            if(is_explosive)
            {
                // implement method per projectile types
                CreateExplosion();
            }

            Destroy(gameObject);
	    }
    }

    void CreateExplosion()
    {
        Debug.Log("Boom");
    }

    protected abstract void Instantiate();
}
