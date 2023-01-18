using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "MyScriptables/WeaponData")]
public class WeaponData : ScriptableObject
{
	public string displayName;
	public GameObject projectile;
	public float recoil;
	public float attackInterval;
	public int ammoCapacity;
	public AudioClip attackSound;
	public AudioClip emptyWeaponSound;
	public AudioClip reloadSound;
	public float reloadTime;
	public bool isAutomatic;
	public GameObject modelPrefab;
}
