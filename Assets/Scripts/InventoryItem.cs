using UnityEngine;

public class InventoryItem
{
    public Actor owningActor;
    public int amount;
    // temporary, eventually need a better thing like an enum or something.
    public string itemType;
}