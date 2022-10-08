using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public enum CoverType
    {
        Wall,
        Floor
    }

    public CoverType coverType;
    // where the actor should move to when going into this cover
    [HideInInspector]
    public Vector3 actorTargetPosition;

    public void Awake()
    {
        actorTargetPosition = transform.GetChild(0).position;
    }
}
