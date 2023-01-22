using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Actor))]
public class Inventory : MonoBehaviour
{
    [SerializeField] InventoryItemDataStorage dataStorage;
    [SerializeField] WeaponInstance weaponInstance;

    private Equipment equipment;
    private List<InventoryWeapon> weapons = new List<InventoryWeapon>();
    private int currentWeaponIndex;
    private Actor actor;

    private void Awake()
    {
        actor = GetComponent<Actor>();

        InventoryWeapon inventoryWeapon = new InventoryWeapon(dataStorage.pistol);
        inventoryWeapon.itemType = "PISTOL";
        AttemptAddItem(inventoryWeapon);
    }

    public bool AttemptAddItem(InventoryItem item)
    {
        bool result = true;
        try
        {
            switch (item.itemType)
            {
                case "BOMB":
                    if (equipment == null)
                    {
                        Equipment eqItem = (Equipment)item;
                        equipment = eqItem;
                        equipment.owningActor = actor;
                    }
                    break;
                case "RIFLE":
                    weapons.Add((InventoryWeapon)item);
                    weaponInstance.UpdateWeapon((InventoryWeapon)item);
                    break;
                case "PISTOL":
                    weapons.Add((InventoryWeapon)item);
                    weaponInstance.UpdateWeapon((InventoryWeapon)item);
                    break;
            }

            item.amount += item.amount;
        }
        catch
        {
            result = false;
        }

        return result;
    }

    public bool AttemptUseEquipment()
    {
        if (equipment != null)
        {
            return equipment.Use();
        }

        return false;
    }

    public bool AttemptUseWeapon(bool triggerPull)
    {
        if (weaponInstance != null)
        {
            weaponInstance.InitiateAttack(actor.data.recoilControl, triggerPull);
        }

        return false;
    }

    public bool AttemptStartReload()
    {
        // non-players for now have no backup ammo limit.
        if (weaponInstance != null && (!actor.IsPlayer || weaponInstance.inventoryWeapon.amount > 0))
        {
            weaponInstance.StartReload();
        }

        return true;
    }

    public bool AttemptSwitchWeapons()
    {
        if (weaponInstance != null && weapons.Count > 1)
        {
            currentWeaponIndex += 1;
            if (currentWeaponIndex >= weapons.Count)
            {
                currentWeaponIndex = 0;
            }

            weaponInstance.UpdateWeapon(weapons[currentWeaponIndex]);

            return true;
        }

        return false;
    }

    public int GetEquippedWeaponAmmo()
    {
        if (weaponInstance != null)
        {
            return weaponInstance.GetAmmo();
        }

        return 0;
    }
}
