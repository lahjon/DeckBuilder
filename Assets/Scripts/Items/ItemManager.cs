using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemManager : Manager
{
    public GameObject itemPrefab;
    public Canvas canvas;
    public Transform content;
    public List<ItemData> allItems = new List<ItemData>(); 
    public List<ItemData> equippedItems = new List<ItemData>(); 
    public int maxItemSlots;
    public int usedItemSlots;
    protected override void Awake()
    {
        base.Awake();
        world.itemManager = this;
    }
    protected override void Start()
    {
        base.Start();
    }

    ItemData GetRandomItemData()
    {
        ItemData[] items = allItems.Except(equippedItems).ToArray();
        if (items.Count() > 0)
            return items[Random.Range(0, items.Count())];

        return null;
    }

    public void RemoveItem(Item item)
    {
        if (equippedItems.Contains(item.itemData))
        {
            equippedItems.Remove(item.itemData);
            Destroy(item);
        }
    }

    public void EquipRandomItem()
    {
        if (usedItemSlots >= maxItemSlots)
        {
            Debug.LogWarning("No free item slots!");
            return;
        }

        ItemData data = GetRandomItemData();
        usedItemSlots++;
        equippedItems.Add(data);
        Item newItem = Instantiate(itemPrefab, content).GetComponent<Item>();
        newItem.itemData = data;
        newItem.AddItem();
    }
}
