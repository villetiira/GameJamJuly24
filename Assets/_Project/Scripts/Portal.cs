using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : Interactable
{
    [Header("Portal specifics")]
    public bool portalInShop = false;
    public Portal connectedPortal;
    public Transform exitPoint;

    private void Start()
    {
        if (!portalInShop)
        {
            Portal[] portals = FindObjectsByType<Portal>(0);
            foreach (Portal portal in portals)
            {
                if(portal.portalInShop)
                {
                    portal.ConnectToPortal(this);
                    ConnectToPortal(portal);
                }
            }
        }
    }

    public void ConnectToPortal(Portal portal)
    {
        connectedPortal = portal;
    }

    public override void Interact(GameObject player)
    {
        player.transform.position = connectedPortal.exitPoint.position;
        player.transform.rotation = connectedPortal.exitPoint.localRotation;
    }

}
