using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace keijo
{
    public class AlchemyStation : Interactable
    {
        public GameObject AlchemyGUI;

        public override void Interact(GameObject player)
        {
            AlchemyGUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
