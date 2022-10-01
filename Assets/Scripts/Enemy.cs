 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
	//[SerializeField] Transform torsoT;
	
	private NavMeshAgent navAgent;

	private GameObject target;

	public GameObject upperBody;

	private bool playerInSights;
	private bool pauseFiring;

	public Weapon weapon;

	public PlayerData data;

	private void Start()
	{
		// required settings for 2D navmesh to work correctly
		//navAgent = GetComponent<NavMeshAgent>();
		//navAgent.updateRotation = false;
		//navAgent.updateUpAxis = false;

		// temporary
		target = GameObject.FindObjectOfType<Player>().gameObject;
	}

    private void Update()
    {
		Vector3 direction = (target.transform.position - transform.position).normalized;

		UpdateAim(direction);

		if (!weapon.HasAmmo())
		{
			weapon.StartReload();
		} 
		else if (playerInSights && !pauseFiring)
        {

			int numToFire = Random.Range(1, 7);

			StartCoroutine(FireBurst(numToFire));
        }

		// chase player
		//navAgent.destination = FindObjectOfType<Player>().transform.position;

		//// update aim rotation
		//Quaternion q = new Quaternion();
		//Vector3 v3 = navAgent.path.corners[1];
		//torsoT.right = navAgent.path.corners[1] - transform.position;
    }

	private IEnumerator FireBurst(int numToFire)
    {
		pauseFiring = true;

		int initialWeaponAmmo = weapon.GetAmmo();
		int currentWeaponAmmo = initialWeaponAmmo;

		while (initialWeaponAmmo - currentWeaponAmmo < numToFire && currentWeaponAmmo > 0)
        {
            weapon.InitiateAttack(data.recoilControl);
			currentWeaponAmmo = weapon.GetAmmo();

			yield return null;
        }


		yield return new WaitForSeconds(.5f);

		pauseFiring = false;
	}

	protected void UpdateAim(Vector2 aimVector)
	{
		// normalized direction to shoot the projectile
		//aimVector = (reticle.position - transform.position).normalized;

		// no idea what this math is.
		float angle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

		// if we cross from 360 - 0 or the other way around, handle it
		bool crossedZeroDown = rotation.eulerAngles.z > 180 && upperBody.transform.rotation.eulerAngles.z < 90;
		bool crossedZeroUp = rotation.eulerAngles.z < 90 && upperBody.transform.rotation.eulerAngles.z > 180;

		if (Mathf.Abs(rotation.eulerAngles.z - upperBody.transform.rotation.eulerAngles.z) < 10)
		{
			upperBody.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
			upperBody.GetComponent<Rigidbody>().MoveRotation(rotation);

			playerInSights = true;

			return;
		}
		else if (!crossedZeroDown && rotation.eulerAngles.z > upperBody.transform.rotation.eulerAngles.z || crossedZeroUp)
		{
			upperBody.GetComponent<Rigidbody>().AddTorque(Vector3.forward * data.rotationTorque);
		}
		else
		{
			upperBody.GetComponent<Rigidbody>().AddTorque(Vector3.forward * -data.rotationTorque);
		}

		playerInSights = false;
	}
}
