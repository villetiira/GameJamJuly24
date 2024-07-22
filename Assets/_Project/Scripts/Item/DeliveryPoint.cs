using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace keijo {
    public class DeliveryPoint : Interactable
    {
        GameManager gameManager;

        private void Awake()
        {
            gameManager = FindFirstObjectByType<GameManager>();
        }

        public override void Interact(GameObject player)
        {
            Inventory inventory = player.GetComponent<Inventory>();
            int blueCaps = inventory.blueCaps;
            int glowBuds = inventory.glowBuds;
            int crystals = inventory.crystals;
            int paleShrooms = inventory.paleShrooms;
            inventory.ClearInventory();

            Debug.Log("Delivered mushrooms: " + blueCaps);

            gameManager.DeliverItems(blueCaps, glowBuds, crystals, paleShrooms);
        }
    }
}
