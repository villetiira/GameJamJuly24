using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public string interactableName;
    public float interactTime = 2f;
    public string interactTooltip;

    public virtual void Interact(GameObject player)
    {
        Debug.Log("Interacting with a " + interactableName);
    }
}
