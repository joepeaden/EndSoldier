using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "MyScriptables/ProjectileData")]
public class ProjectileData : ScriptableObject
{
    public float velocity;
    public int damage;
    public Material material;
    //public float impact; //75?
    //public float range;
    public bool isExplosive;
}
