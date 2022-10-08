using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActorCoverSensor : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnCoverNearby;

    private Cover cover;

    private void OnTriggerEnter(Collider other)
    {
        Cover cover = other.GetComponent<Cover>();
        if (cover)
        {
            this.cover = cover;
            OnCoverNearby.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Cover cover = other.GetComponent<Cover>();
        if (cover)
        {
            this.cover = null;
        }
    }

    public Cover GetCover()
    {
        return cover;
    }
}
