using UnityEngine;
using UnityEngine.Events;

public class Inventory : MonoBehaviour
{
    public int blueCaps;
    public int glowBuds;
    public int crystals;
    public int paleShrooms;
    public int hotSauce;
    public int manaPotion;
    public int elixirOfProtection;
    public int healthPotion;

    public UnityEvent InventoryUpdated = new UnityEvent();

    public void AddItem(string itemName)
    {
        switch(itemName)
        {
            case "Bluecap":
                blueCaps++;
                break;
            case "Glowbud":
                glowBuds++;
                break;
            case "Crystal":
                crystals++;
                break;
            case "Paleshroom":
                paleShrooms++;
                break;
            case "Hot Sauce":
                hotSauce++;
                break;
            case "Mana Potion":
                manaPotion++;
                break;
            case "Elixir of Protection":
                elixirOfProtection++;
                break;
            case "Health Potion":
                healthPotion++;
                break;
        default:
                Debug.Log("Tried adding an item not yet listed in inventory: " + itemName);
                break;
        }
    }

    public int GetAmount(string itemName)
    {
        switch (itemName)
        {
            case "Bluecap":
                return blueCaps;
            case "Glowbud":
                return glowBuds;
            case "Crystal":
                return crystals;
            case "Paleshroom":
                return paleShrooms;
            default:
                Debug.Log("Tried adding an item not yet listed in inventory: " + itemName);
                return 0;
        }
    }

    public void RemoveItem(string itemName)
    {
        switch (itemName)
        {
            case "Bluecap":
                blueCaps--;
                break;
            case "Glowbud":
                glowBuds--;
                break;
            case "Crystal":
                crystals--;
                break;
            case "Paleshroom":
                paleShrooms--;
                break;
            default:
                Debug.Log("Tried adding an item not yet listed in inventory: " + itemName);
                break;
        }
        InventoryUpdated.Invoke();
    }

    public void ClearInventory()
    {
        hotSauce = 0;
        manaPotion = 0;
        elixirOfProtection = 0;
        healthPotion = 0;
        InventoryUpdated.Invoke();
    }
}
