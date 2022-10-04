using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Author: Joseph Peaden

/// <summary>
/// Just holds enum that clarifies what layers are what
/// </summary>
public class IgnoreLayerCollisions : MonoBehaviour
{
    public enum CollisionLayers
    {
        Default = 0,
        InCover = 7,
        Projectiles = 8
    }
}
