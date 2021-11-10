using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ItemUseableManager : Manager, ISaveableTemp
{
    public GameObject itemPrefab;
    public Canvas canvas;
    public Transform content;
    public List<ItemUseableData> allItems = new List<ItemUseableData>(); 
    public List<ItemUsable> equippedItems = new List<ItemUsable>(); 
    public int maxItemSlots;
    public int usedItemSlots;
    protected override void Awake()
    {
        base.Awake();
        world.itemUseableManager = this;
    }
    protected override void Start()
    {
        base.Start();
    }

    public ItemUseableData GetItemData(int anId = -1)
    {
        if (anId >= 0)
            return allItems.Except(equippedItems.Select(x => x.itemData)).FirstOrDefault(x => x.itemId == anId);
        else
        {
            ItemUseableData[] items = allItems.Except(equippedItems.Select(x => x.itemData)).ToArray();
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

    public void AddItem(int anId = -1)
    {
        if (usedItemSlots >= maxItemSlots)
        {
            Debug.LogWarning("No free item slots!");
            return;
        }

        ItemUseableData data;

        if (anId >= 0)
            data = allItems.Except(equippedItems.Select(x => x.itemData)).FirstOrDefault(x => x.itemId == anId);
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
        a_SaveData.selectedUseItems = equippedItems.Select(x => x.itemData.itemId).ToList();
    }

    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.selectedUseItems.ForEach(x => AddItem(x));
    }
}
