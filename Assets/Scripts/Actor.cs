using System.Collections;
using UnityEngine;
// the player controller.
// things to implement: recoil control value (reduced recoil from weapon)

public class Actor : MonoBehaviour
{
	public virtual void GetHit(int damage)
    {
		;
    }

	//[SerializeField]
	//private ActorData data;
	//[SerializeField]
	//private GameObject upperBody;
	//[SerializeField]
	//private Weapon weapon;

	////// vars local to this class that change ////
	//// recoil lerp value - seems like it's gotta be a class var for some reason. Don't feel like looking into it.
	//private float t = 0.0f;
	//private float moveSpeed;
	//private int hitPoints;

	//private void Start()
	//{
	//	hitPoints = data.hitPoints;
	//}

	

	//protected void UpdateAim(Vector2 aimVector)
	//{
	//	// normalized direction to shoot the projectile
	//	aimVector = (reticle.position - transform.position).normalized;

	//	// no idea what this math is.
	//	float angle = Mathf.Atan2(aimVector.y, aimVector.x) * Mathf.Rad2Deg;
	//	Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);

	//	// if we cross from 360 - 0 or the other way around, handle it
	//	bool crossedZeroDown = rotation.eulerAngles.z > 180 && upperBody.transform.rotation.eulerAngles.z < 90;
	//	bool crossedZeroUp = rotation.eulerAngles.z < 90 && upperBody.transform.rotation.eulerAngles.z > 180;

	//	if (Mathf.Abs(rotation.eulerAngles.z - upperBody.transform.rotation.eulerAngles.z) < 10)
	//	{
	//		upperBody.GetComponent<Rigidbody2D>().angularVelocity = 0;
	//		upperBody.GetComponent<Rigidbody2D>().MoveRotation(rotation);
	//	}
	//	else if (!crossedZeroDown && rotation.eulerAngles.z > upperBody.transform.rotation.eulerAngles.z || crossedZeroUp)
	//	{
	//		upperBody.GetComponent<Rigidbody2D>().AddTorque(data.rotationTorque);
	//	}
	//	else
	//	{
	//		upperBody.GetComponent<Rigidbody2D>().AddTorque(-data.rotationTorque);
	//	}
	//}

	//public void GetHit(int damage)
	//{
	//	hitPoints -= damage;

	//	//UIManager.instance.UpdateHealthBar(hitPoints, data.hitPoints);

	//	Debug.Log(hitPoints);

	//	if (hitPoints <= 0)
	//		Die();
	//}

	//protected void Die()
	//{
	//	FlowManager.instance.GameOver();
	//}

	//private Vector3 GetMoveVector()
	//{
	//	Vector3 moveVector = Vector3.zero;
	//	if (Input.GetKey(KeyCode.W))
	//	{
	//		moveVector += Vector3.up;
	//	}
	//	if (Input.GetKey(KeyCode.S))
	//	{
	//		moveVector += -Vector3.up;
	//	}
	//	if (Input.GetKey(KeyCode.A))
	//	{
	//		moveVector += -Vector3.right;
	//	}
	//	if (Input.GetKey(KeyCode.D))
	//	{
	//		moveVector += Vector3.right;
	//	}

	//	return moveVector;
	//}

	//private void Reload()
	//{

	//}
}
