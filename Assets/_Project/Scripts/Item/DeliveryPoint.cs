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
            int blueCaps = player.GetComponent<Inventory>().blueCaps;
            int glowBuds = player.GetComponent<Inventory>().glowBuds;
            int crystals = player.GetComponent<Inventory>().crystals;
            int paleShrooms = player.GetComponent<Inventory>().paleShrooms;

            Debug.Log("Delivered mushrooms: " + blueCaps);

            gameManager.DeliverItems(blueCaps, glowBuds, crystals, paleShrooms);
        }
    }
}
