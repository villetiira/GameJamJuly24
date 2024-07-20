using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string type;

    public virtual void Interact(GameObject player)
    {
        Debug.Log("Interacting with a " + type);
    }
}
