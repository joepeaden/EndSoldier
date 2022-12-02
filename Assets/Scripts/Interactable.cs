using UnityEngine;

/// <summary>
/// For objects actors can interact with.
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    public abstract void Interact();

    public void PromptForInteraction(Actor a, bool canInteract)
    {
        // later use a to alert actor that interaction is available.
        if (canInteract)
        {
            Debug.Log("Interaction available, press E.");
        }
        else
        {
            Debug.Log("Interation no longer available");
        }    
    }

    public void OnTriggerEnter(Collider other)
    {
        Actor a = other.GetComponent<Actor>();
        if (a)
        {
            PromptForInteraction(a, true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Actor a = other.GetComponent<Actor>();
        if (a)
        {
            PromptForInteraction(a, false);
        }
    }
}
