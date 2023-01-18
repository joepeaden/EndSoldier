using UnityEngine;

[CreateAssetMenu(fileName = "ExplosiveData", menuName = "MyScriptables/ExplosiveData")]
public class ExplosiveData : ScriptableObject
{
    public float explosionRadius;
    public float explosionPower;
    public float upwardsForce;
    public GameObject explosionPrefab;
}
