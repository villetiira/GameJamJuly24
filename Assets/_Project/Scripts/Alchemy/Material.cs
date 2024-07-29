using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace keijo
{
    public class Material : MonoBehaviour, IPointerDownHandler
    {
        public string materialName = "";
        public TMP_Text amount;
        public Image icon;
        Inventory inventory;
        Alchemy alchemy;

        private void Start()
        {
            inventory = FindFirstObjectByType<Inventory>();
            alchemy = FindFirstObjectByType<Alchemy>();
            inventory.InventoryUpdated.AddListener(UpdateAmount);
        }
        public void OnPointerDown(PointerEventData eventData)
        {
            SelectItem();
        }

        public void SelectItem()
        {
            if(!alchemy) alchemy = FindFirstObjectByType<Alchemy>();
            if(inventory.GetAmount(materialName) > 0)
            {
                alchemy.AddIngredient(materialName, icon.sprite);
            }
        }

        void OnEnable()
        {
            UpdateAmount();
        }

        public void UpdateAmount()
        {
            amount.text = inventory.GetAmount(materialName).ToString();
        }

    }
}
