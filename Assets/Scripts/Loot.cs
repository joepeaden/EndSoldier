using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Instance of loot for actor to pick up.
/// </summary>
public class Loot : Interactable, ISetActive
{
    [SerializeField] private InventoryItemDataStorage dataStorage;

    public InventoryItem item;
    // temporary, eventually will do a dropdown to select the loot type.
    public string rewardKey;

    private MeshRenderer rend;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        switch (rewardKey)
        {
            case "BOMB":
                item = new ExplosiveEquipment(dataStorage.plasticExplosive);
                break;
            case "RIFLE":
                item = new InventoryWeapon(dataStorage.assaultRifle);
                break;
            case "SMG":
                item = new InventoryWeapon(dataStorage.subMachineGun);
                break;
            case "PISTOL":
                item = new InventoryWeapon(dataStorage.pistol);
                break;
        }

        item.rewardKey = rewardKey;
    }

    public override void Interact(Actor a)
    {
        base.Interact(a);

        a.PickupLoot(this);

        Destroy(gameObject);
    }

    void ISetActive.Activate()
    {
        rend.enabled = true;   
    }

    void ISetActive.DeActivate()
    {
        rend.enabled = false;
    }
}
