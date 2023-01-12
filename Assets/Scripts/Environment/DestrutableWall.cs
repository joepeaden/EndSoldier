using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Shatter))]
public class DestrutableWall : Interactable
{
    private Shatter shatter;
    public GameObject bomb;

    private void Start() {
        shatter = GetComponent<Shatter>();
    }

    public override void Interact(Actor a)
    {
        base.Interact(a);
        
        Vector3 actorPos = interactingActor.transform.position;
        Instantiate(bomb, actorPos, Quaternion.identity);
    }
}
