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
    public string lootType;

    private MeshRenderer rend;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        switch (lootType)
        {
            case "BOMB":
                item = new ExplosiveEquipment(dataStorage.plasticExplosive);
                break;
            case "RIFLE":
                item = new InventoryWeapon(dataStorage.assaultRifle);
                break;
            case "PISTOL":
                item = new InventoryWeapon(dataStorage.pistol);
                break;
        }

        item.itemType = lootType;
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
