using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The logical representation of an explosive for inventory.
/// </summary>
public class ExplosiveEquipment : Equipment
{
    public ExplosiveData data;

    public ExplosiveEquipment(ExplosiveData data)
    {
        this.data = data;
        amount = data.totalAmount;
    }

    public override bool Use()
    {
        if (amount > 0)
        {
            try
            {
                Vector3 actorPos = owningActor.transform.position;
                ExplosiveInstance inst = GameObject.Instantiate(data.instancePrefab, actorPos, Quaternion.identity).GetComponent<ExplosiveInstance>();
                inst.data = data;
                amount--;

                return true;
            }
            catch
            {
                return false;
            }
        }

        return false;
    }

}
