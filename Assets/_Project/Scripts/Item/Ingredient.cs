using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : Interactable
{
    public string itemName;
    public float gatherTime;

    public override void Interact(GameObject player)
    {
        player.GetComponent<Inventory>().AddItem(itemName);
        Destroy(gameObject);
    }
}
