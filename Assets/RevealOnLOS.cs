using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealOnLOS : MonoBehaviour
{
    public Material coverInLOS;
    public Material coverOutLOS;

    public void Reveal()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = coverInLOS;
    }

    public void Hide()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = coverOutLOS;
    }
}
