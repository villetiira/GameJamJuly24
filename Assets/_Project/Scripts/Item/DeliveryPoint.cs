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

            int hotSauce = inventory.hotSauce;
            int manaPotion = inventory.manaPotion;
            int elixirOfProtection = inventory.elixirOfProtection;
            int healthPotion = inventory.healthPotion;
            inventory.ClearInventory();

            gameManager.DeliverItems(hotSauce, manaPotion, elixirOfProtection, healthPotion);
        }
    }
}
