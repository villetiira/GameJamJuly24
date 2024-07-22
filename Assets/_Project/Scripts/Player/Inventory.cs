using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int blueCaps;
    public int glowBuds;
    public int crystals;
    public int paleShrooms;

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
            default:
                Debug.Log("Tried adding an item not yet listed in inventory: " + itemName);
                break;
        }
    }

    public void ClearInventory()
    {
        blueCaps = 0;
        glowBuds = 0;
        crystals = 0;
        paleShrooms = 0;
    }
}
