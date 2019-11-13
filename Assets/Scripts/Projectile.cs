using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
	public float speed;

	// public enum Team { Friendly, Hostile };
	// private Team team;

    void Update()
    {
        transform.Translate(Vector3.right);
    }

    // public void UpdateTeam(Team team)
    // {
    // 	this.team = team;
    // 	// if(team == 1)
    // 	// 	this.team = Team.Friendly;
    // 	// else
    // 	// 	this.team = Team.Hostile; 
    // }

    void OnTriggerEnter2D(Collider2D other)
    {
    	Actor actor = other.GetComponent<Actor>();
    	if(actor != null)
	    {
	    	if(actor is Hostile)
    		{
    			actor.GetHit();
	    		Destroy(gameObject);
	    	}	
	    }
    }
}
