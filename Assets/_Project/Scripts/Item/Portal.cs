using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace keijo
{
    public class Portal : Interactable
    {
        [Header("Portal specifics")]
        public bool portalInShop = false;
        public Portal connectedPortal;
        public Transform exitPoint;
        public Light directionalLight;
        public bool indoors = true;
        public GameManager gameManager;

        private void Start()
        {
            if (!portalInShop)
            {
                Portal[] portals = FindObjectsByType<Portal>(0);
                foreach (Portal portal in portals)
                {
                    if (portal.portalInShop)
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

            SwitchLights();
            SwitchAudio();
        }

        void SwitchLights()
        {
            if (directionalLight == null)
            {
                directionalLight = GameObject.Find("Directional Light").GetComponent<Light>();
            }
            directionalLight.enabled = indoors; // if the door is inside, enable the light
            RenderSettings.fog = !indoors;
        }

        void SwitchAudio()
        {
            if(!gameManager)
            {
                gameManager = FindFirstObjectByType<GameManager>();
            }
            if(indoors)
            {
                gameManager.backgroundMusic.volume = 100;
                gameManager.dungeonBackground.volume = 0;
            }
            else
            {
                gameManager.dungeonBackground.volume = 100;
                gameManager.backgroundMusic.volume = 0;
            }
        }

    }
}
