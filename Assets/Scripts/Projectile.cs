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
    protected bool is_explosive;
 
    void Start()
    {
        // Instantiate();
    }

    void Update()
    {
        if (force != 0)
            GetComponent<Rigidbody>().AddForce(force * transform.right);
        else
            Debug.LogWarning("Projectile force for " + gameObject.name + " is not set");

        // record distance travelled, delete if outside range
    }

    void OnCollisionEnter(Collision other)
    {
    	Actor actor = other.gameObject.GetComponent<Actor>();
    	if(actor != null)
	    {
            //actor.GetComponent<Rigidbody>().AddForce(transform.right * impact);
            actor.GetHit(damage);

            if(is_explosive)
            {
                // implement method per projectile types
                CreateExplosion();
            }

        }

        Destroy(gameObject);
    }

    protected void CreateExplosion()
    {
        Debug.Log("Boom");
    }

    // protected abstract void Instantiate();
}
