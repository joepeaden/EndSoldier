using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeRangefinder : MonoBehaviour
{

	public Collider2D targetZone;
	public GameObject parent;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			parent.GetComponent<Hostile>().TargetInRange(true);
		}
	}


	void OnTriggerExit2D(Collider2D other)
	{
		if(other.tag == "Player")
		{
			parent.GetComponent<Hostile>().TargetInRange(false);
		}
	}

}
