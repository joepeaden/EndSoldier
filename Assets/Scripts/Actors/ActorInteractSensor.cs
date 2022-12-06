using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActorInteractSensor : MonoBehaviour
{
    [HideInInspector]
    public UnityEvent OnInteractableNearby;

    private Interactable interactable;

    private void OnTriggerEnter(Collider other)
    {
        Interactable i = other.GetComponent<Interactable>();
        if (i)
        {
            interactable = i;
            OnInteractableNearby.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Interactable i = other.GetComponent<Interactable>();
        if (i)
        {
            this.interactable = null;
        }
    }

    public Interactable GetInteractable()
    {
        return interactable;
    }
}
