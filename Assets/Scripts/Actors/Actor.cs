using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
	[SerializeField]
	protected Animator anim;
	[SerializeField]
	protected float moveSpeed;
	[SerializeField]
	protected GameObject upperBody;
	protected int hitPoints;
	protected int maxHitPoints;
	[SerializeField]
	protected Weapon weapon;
 

	protected abstract void UpdateAim(Vector2 targetPos);

	public abstract void GetHit(int damage);

	protected abstract void Die();

	IEnumerator SpriteColorFlash()
    {
		SpriteRenderer renderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
	    Color originalColor = renderer.color;

		renderer.color = Color.red;

		yield return new WaitForSeconds(0.2f);
		renderer.color = originalColor;
		yield return new WaitForSeconds(0.2f);
		renderer.color = Color.red;
		yield return new WaitForSeconds(0.2f);
		renderer.color = originalColor;
		yield return new WaitForSeconds(0.2f);
		renderer.color = Color.red;
		yield return new WaitForSeconds(0.2f);
		renderer.color = originalColor;
	}
}
