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
    [SerializeField] private List<GameObject> actorTargetPositionGOList = new List<GameObject>();

    public void Awake()
    {
        // disable actor target position mesh during play
        foreach (GameObject targetPosGO in actorTargetPositionGOList)
        {
            targetPosGO.SetActive(false);
        }
    }

    public Vector3 GetActorCoverPosition(Vector3 actorPos)
    {
        // get closest target position, that's where actor will go
        Vector3 targetPos = actorPos;
        float minDist = int.MaxValue;
        foreach (GameObject targetPosGo in actorTargetPositionGOList)
        {
            float dist = Mathf.Abs((targetPosGo.transform.position - actorPos).magnitude);
            if (dist < minDist)
            {
                minDist = dist;
                targetPos = targetPosGo.transform.position;
            }
        }

        // nullify Y value (don't want actor moving up)
        targetPos.y = 0f;

        return targetPos;
    }

    /// <summary>
    /// Get the position for the actor to transfer (vault) to.
    /// </summary>
    /// <param name="actorPos">Position of the calling actor.</param>
    /// <returns>Position for the actor to transfer to.</returns>
    public Vector3 GetActorFlipPosition(Vector3 actorPos)
    {
        // get furthest target position, that's where actor will go
        Vector3 targetPos = actorPos;
        float minDist = int.MaxValue;
        foreach (GameObject targetPosGo in actorTargetPositionGOList)
        {
            float dist = Mathf.Abs((targetPosGo.transform.position - actorPos).magnitude);
            if (dist > minDist)
            {
                minDist = dist;
                targetPos = targetPosGo.transform.position;
            }
        }

        // nullify Y value (don't want actor moving up)
        targetPos.y = 0f;

        return targetPos;
    }
}
