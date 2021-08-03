using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingShop : Building
{
    public List<GameObject> inventoryAct1 = new List<GameObject>();
    public List<GameObject> inventory = new List<GameObject>();
    public Transform iventoryPanel;

    private int inventorySpace = 3;
    public override void CloseBuilding()
    {
        base.CloseBuilding();
    }
    public override void EnterBuilding()
    {
        base.EnterBuilding();
        UpdateInventory();
    }

    private void AddToShop(GameObject item)
    {
        GameObject newItem = Instantiate(item);
        newItem.transform.SetParent(iventoryPanel);
        inventory.Add(item);
    }
    private void UpdateInventory()
    {
        foreach (GameObject item in inventoryAct1)
        {
            if (!inventory.Contains(item) && !WorldSystem.instance.townManager.unlockedBuildings.Contains(item.GetComponent<ShopItem>().building))
            {
                AddToShop(item);
            }

        }
        while (inventory.Count <= inventorySpace)
        {
            AddToShop(WorldSystem.instance.shopManager.outOfStockPrefab);
        }
    }
}
