using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBolt : Projectile
{
    protected override void Instantiate()
    {
        damage = 2;
        speed = 0.4f;
        impact = 1f;
        // maybe this should be true... attention to detail!
        is_explosive = false;
    }
}
