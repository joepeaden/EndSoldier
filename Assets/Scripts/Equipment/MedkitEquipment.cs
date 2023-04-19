using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The logical representation of an medkit for inventory.
/// Can 
/// </summary>
public class MedkitEquipment : Equipment
{
    public MedkitData medData;

    public MedkitEquipment(MedkitData data)
    {
        this.data = data;
        medData = (MedkitData)data;
        amount = data.totalAmount;
    }

    public override bool Use()
    {
        if (amount > 0)
        {
            try
            {
                owningActor.AddHitPoints(medData.amountHealed);
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
