using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {
    [System.Serializable]
    public struct InventoryItem
    {
        public string name;
        public string internal_name;
        public string description;
        public Sprite image;
        public bool playerHasIt;
    }

    public InventoryItem[] inventoryItems;

    public void PlayerHas(string what)
    {
        bool hasIt = false;
        for (int i = 0; i < inventoryItems.Length; i++)
        {
            if (inventoryItems[i].internal_name == what)
                hasIt = true;
        }
    }
}
