using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UseItemManager : Manager
{
    public GameObject itemPrefab;
    public Canvas canvas;
    public Transform content;
    public List<UseItemData> allItems = new List<UseItemData>(); 
    public List<UseItemData> equippedItems = new List<UseItemData>(); 
    public int maxItemSlots;
    public int usedItemSlots;
    protected override void Awake()
    {
        base.Awake();
        world.useItemManager = this;
    }
    protected override void Start()
    {
        base.Start();
    }

    public UseItemData GetItemData(string value = "")
    {
        if (!string.IsNullOrEmpty(value))
            return allItems.Except(equippedItems).FirstOrDefault(x => x.name == value);
        else
        {
            UseItemData[] items = allItems.Except(equippedItems).ToArray();
            if (items.Count() > 0)
                return items[Random.Range(0, items.Count())];

            return null;
        }
    }

    public void RemoveItem(UseItem item)
    {
        if (equippedItems.Contains(item.itemData))
        {
            equippedItems.Remove(item.itemData);
            Destroy(item);
        }
    }

    public void AddItem(string itemName = "")
    {
        if (usedItemSlots >= maxItemSlots)
        {
            Debug.LogWarning("No free item slots!");
            return;
        }

        UseItemData data;

        if (!string.IsNullOrEmpty(itemName))
            data = allItems.Except(equippedItems).FirstOrDefault(x => x.name == itemName);
        else
            data = GetItemData();

        if (data == null)
            return;

        usedItemSlots++;
        equippedItems.Add(data);
        UseItem newItem = Instantiate(itemPrefab, content).GetComponent<UseItem>();
        newItem.itemData = data;
        newItem.AddItem();
    }
}
