using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class UseItemManager : Manager, ISaveableTemp
{
    public GameObject itemPrefab;
    public Canvas canvas;
    public Transform content;
    public List<UseItemData> allItems = new List<UseItemData>(); 
    public List<ItemUsable> equippedItems = new List<ItemUsable>(); 
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
            return allItems.Except(equippedItems.Select(x => x.itemData)).FirstOrDefault(x => x.name == value);
        else
        {
            UseItemData[] items = allItems.Except(equippedItems.Select(x => x.itemData)).ToArray();
            if (items.Count() > 0)
                return items[Random.Range(0, items.Count())];

            return null;
        }
    }

    public void RemoveItem(ItemUsable item)
    {
        if (equippedItems.Contains(item))
        {
            equippedItems.Remove(item);
            item.RemoveItem();
        }
    }

    public void RemoveItem()
    {
        ItemUsable item = equippedItems[Random.Range(0, equippedItems.Count - 1)];
        if (item != null)
        {
            equippedItems.Remove(item);
            item.RemoveItem();
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
            data = allItems.Except(equippedItems.Select(x => x.itemData)).FirstOrDefault(x => x.name == itemName);
        else
            data = GetItemData();

        if (data == null)
            return;

        usedItemSlots++;
        ItemUsable newItem = Instantiate(itemPrefab, content).GetComponent<ItemUsable>();
        equippedItems.Add(newItem);
        newItem.itemData = data;
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.selectedUseItems = equippedItems.Select(x => x.itemData.name).ToList();
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.selectedUseItems.ForEach(x => AddItem(x));
    }
}
