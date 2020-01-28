using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
	protected float speed;
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
            actor.GetComponent<Rigidbody2D>().AddForce(transform.right * impact);
            actor.GetHit(damage);

            if(is_explosive)
            {
                // implement method per projectile types
                CreateExplosion();
            }

            Destroy(gameObject);
	    }
    } 

    protected void CreateExplosion()
    {
        Debug.Log("Boom");
    }

    // protected abstract void Instantiate();
}
