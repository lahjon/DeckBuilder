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

    UseItemData GetRandomItemData()
    {
        UseItemData[] items = allItems.Except(equippedItems).ToArray();
        if (items.Count() > 0)
            return items[Random.Range(0, items.Count())];

        return null;
    }

    public void RemoveItem(UseItem item)
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

        UseItemData data = GetRandomItemData();
        usedItemSlots++;
        equippedItems.Add(data);
        UseItem newItem = Instantiate(itemPrefab, content).GetComponent<UseItem>();
        newItem.itemData = data;
        newItem.AddItem();
    }
}
