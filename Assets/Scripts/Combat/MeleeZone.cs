using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeZone : MonoBehaviour
{

	public static List<Hostile> targets;

	void Start()
	{
		targets = new List<Hostile>();
	} 

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.tag == "Enemy")
		{
			targets.Add(other.GetComponent<Hostile>());
		}
	}

	void OnTriggerExit2D(Collider2D other)
	{
		if(other.gameObject.tag == "Enemy")
		{
			targets.Remove(other.GetComponent<Hostile>());
		}
	}
}
