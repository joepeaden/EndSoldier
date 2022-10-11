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
    [SerializeField] private GameObject actorTargetPositionGO;

    public void Awake()
    {
        // disable actor target position mesh during play
        actorTargetPositionGO.SetActive(false);
    }

    public Vector3 GetActorTargetPosition()
    {
        Vector3 targetPos = actorTargetPositionGO.transform.position;
        // nullify Y value (don't want actor moving up)
        targetPos.y = 0f;

        return targetPos;
    }
}
