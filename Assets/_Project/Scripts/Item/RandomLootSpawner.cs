using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace keijo
{
    public class RandomLootSpawner : MonoBehaviour
    {
        public List<GameObject> spawnableItemList = new List<GameObject>();
        public bool itemSpawned = false;

        void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "MyIcon", true);
        }
    }
}
