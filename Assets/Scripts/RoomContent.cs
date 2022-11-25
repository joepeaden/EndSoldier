using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For any decoration, object, or furniture in a room that needs to be toggled when the room is entered or exited.
/// </summary>
public class RoomContent : MonoBehaviour
{
    [SerializeField] private Material transparentMaterial;

    private Material[] defaultMaterials;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        defaultMaterials = meshRenderer.materials;

        SetTransparency(true);
    }

    /// <summary>
    /// Set all the materials for the MeshRenderer to the transparent material, or the opposite.
    /// </summary>
    /// <param name="shouldBeTransparent"></param>
    public void SetTransparency(bool shouldBeTransparent)
    {
        if (shouldBeTransparent)
        {
            int len = meshRenderer.materials.Length;
            Material[] newMat = new Material[len];
            
            for (int i = 0; i < len; i++)
            {
                newMat[i] = transparentMaterial;
            }

            meshRenderer.materials = newMat;
        }
        else
        {
            meshRenderer.materials = defaultMaterials;
        }
    }
}
