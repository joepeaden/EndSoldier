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
        Actors = 3,
        InCover = 7,
        Projectiles = 8,
        Furniture = 9,
        DoorsWhenOpen = 10,
        SightBlocking = 11
    }
}
