using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace keijo
{
    public class Alchemy : MonoBehaviour
    {
        public string ingredient1 = "";
        public string ingredient2 = "";
        public string potion = "";

        [Header("GUI")]
        public TMP_Text ingredient1Text;
        public Image ingredient1Icon;
        public TMP_Text ingredient2Text;
        public Image ingredient2Icon;
        public TMP_Text resultText;
        public Image resultIcon;
        public Sprite manaPotion;
        public Sprite healthPotion;
        public Sprite hotSauce;
        public Sprite elixirOfProtection;

        public Inventory inventory;

        private void Start()
        {
            inventory = FindFirstObjectByType<Inventory>();
        }


        Dictionary<string, string> combinationResults = new Dictionary<string, string>
        {
            { "Bluecap_Glowbud", "Hot Sauce" },
            { "Bluecap_Paleshroom", "Mana Potion" },
            { "Glowbud_Crystal", "Elixir of Protection" },
            { "Crystal_Paleshroom", "Health Potion" },
        };

        public void Create()
        {
            if(potion != "")
            {
                // Remove items from inventory
                // Add result into inventory
                inventory.RemoveItem(ingredient1);
                inventory.RemoveItem(ingredient2);
                inventory.AddItem(potion);

                // Remove items from GUI
                ClearFirstSlot();
                ClearSecondSlot();
                ClearResult();
            }

            
        }

        public void AddIngredient(string ingredientName, Sprite icon)
        {
            Debug.Log("selected " + ingredientName);
            // if the ingredient is already added, ignore
            if (ingredientName == ingredient1 || ingredientName == ingredient2)
            {
                return;
            }

            if (ingredient1 == "")
            {
                ingredient1 = ingredientName;
                // change text & icon
                ingredient1Text.text = ingredientName;
                ingredient1Icon.sprite = icon;
            }
            else if (ingredient2 == "")
            {
                ingredient2 = ingredientName;
                // change text & icon
                ingredient2Text.text = ingredientName;
                ingredient2Icon.sprite = icon;
            }
            // if both filled
            else return;

            if(ingredient1 != "" && ingredient2 != "")
            {
                Debug.Log("both slots filled.");
                potion = GetCombination(ingredient1, ingredient2);
                
                DisplayResult();
            }
        }

        public void RemoveItem(int slotNumber)
        {
            if(slotNumber == 1)
            {
                ClearFirstSlot();
            }
            else if(slotNumber == 2)
            {
                ClearSecondSlot();
            }
            ClearResult();
        }

        void ClearFirstSlot()
        {
            ingredient1 = "";
            ingredient1Text.text = "";
            ingredient1Icon.sprite = null;
        }

        void ClearSecondSlot()
        {
            ingredient2 = "";
            ingredient2Text.text = "";
            ingredient2Icon.sprite = null;
        }

        void ClearResult()
        {
            resultIcon.sprite = null;
            resultText.text = "";
        }


        public string GetCombination(string ingredient1, string ingredient2)
        {
            string key1 = $"{ingredient1}_{ingredient2}";
            string key2 = $"{ingredient2}_{ingredient1}";

            if(combinationResults.TryGetValue(key1, out string result))
            {
                return result;
            }

            if (combinationResults.TryGetValue(key2, out result))
            {
                return result;
            }

            return "";
        }

        public void DisplayResult()
        {
            Debug.Log("Results will show now.");
            // change text & icon
            resultText.text = potion;
            switch(potion)
            {
                case "Mana Potion":
                    resultIcon.sprite = manaPotion;
                    break;
                case "Hot Sauce":
                    resultIcon.sprite = hotSauce;
                    break;
                case "Elixir of Protection":
                    resultIcon.sprite = elixirOfProtection;
                    break;
                case "Health Potion":
                    resultIcon.sprite = healthPotion;
                    break;
            }
        }

        public void CloseUI()
        {
            gameObject.SetActive(false);

            // Remove items from GUI
            ClearFirstSlot();
            ClearSecondSlot();
            ClearResult();
            
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
