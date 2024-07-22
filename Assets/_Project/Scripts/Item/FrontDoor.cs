using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace keijo
{
    public class FrontDoor : Interactable
    {
        public GameManager gameManager;
        public override void Interact(GameObject player)
        {
            gameManager.EndDay();
        }
    }
}
