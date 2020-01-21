using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bolt : Projectile
{
    protected override void Instantiate()
    {
        damage = 1;
        speed = 1f;
        impact = 1f;
        // maybe this should be true... attention to detail!
        is_explosive = false;
    }
}
