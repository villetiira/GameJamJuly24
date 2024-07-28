using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace keijo
{
    public class Alchemy : MonoBehaviour
    {
        public string ingredient1 = "";
        public string ingredient2 = "";

        Dictionary<string, string> combinationResults = new Dictionary<string, string>
        {
            { "Bluecap_Glowbud", "Potion of Radiant Vision" },
            { "Bluecap_Crystal", "Elixir of Enhanced Clarity" },
            { "Bluecap_Paleshroom", "Tonic of Resilience" },
            { "Glowbud_Crystal", "Serum of Luminescent Protection" },
            { "Glowbud_Paleshroom", "Potion of Glimmering Health" },
            { "Crystal_Paleshroom", "Elixir of Shaded Stealth" }
        };

        public void Create()
        {
            // Remove items from GUI
            // Remove items from inventory
            // Add result into inventory
            
        }

        public void AddIngredient(string ingredientName)
        {
            if (ingredient1 == "")
            {
                ingredient1 = ingredientName;
                // change text
                // change icon
            }
            else if (ingredient2 == "")
            {
                ingredient2 = ingredientName;
                // change text
                // change icon
            }
            else return;

            if(ingredient1 != "" && ingredient2 != "")
            {
                string result = GetCombination(ingredient1, ingredient2);
                DisplayResult(result);
            }
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

        public void DisplayResult(string result)
        {
            // change icon
            // change text
        }
    }
}
