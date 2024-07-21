using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontDoor : Interactable
{
    public override void Interact(GameObject player)
    {
        Debug.Log("Sorry, day is not over yet");
    }
}
