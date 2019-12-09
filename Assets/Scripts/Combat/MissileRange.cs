using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileRange : MonoBehaviour
{
	public GameObject parent;
	
	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			parent.GetComponent<Shoota>().TargetInRange();			
		}
	}


	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			// this could definetly be abstracted to actor class
			// parent.GetComponent<Actor>()... etc for use with all actors
			parent.GetComponent<Shoota>().TargetOutRange();
		}
	}

}
