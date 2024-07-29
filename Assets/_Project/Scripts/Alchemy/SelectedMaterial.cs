using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace keijo
{
    public class SelectedMaterial : MonoBehaviour, IPointerDownHandler
    {
        public int slotNumber;
        Alchemy alchemy;

        private void Start()
        {
            alchemy = FindFirstObjectByType<Alchemy>();
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            alchemy.RemoveItem(slotNumber);
        }

    }
}
