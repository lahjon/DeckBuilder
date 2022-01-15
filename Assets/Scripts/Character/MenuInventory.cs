using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class MenuInventory : MonoBehaviour
{
    public Transform stashContent, dragParent;
    public GameObject itemSlotPrefab;
    public List<GearDisplay> gearDisplays;
    public List<EquipmentSlot> equipmentSlots;
    public List<GearSlot> gearSlots;
    public EquipmentSlot currentHoveredEquipmentSlot;
    public Equipment currentHoveredEquipment;
    public int usedInventorySlots;
    public int inventorySlots = 52;

    public List<PerkData> allPerkDatas = new List<PerkData>();
    public List<PerkData> allEquippedPerksDatas = new List<PerkData>();
    public List<Perk> allEquippedPerks = new List<Perk>();
    public Transform perkContent;
    public GameObject perkPrefab;

    public void Init()
    {
        allEquippedPerksDatas.ForEach(x => CreatePerk(x));
        allEquippedPerksDatas.Clear();
    }
    public PerkData GetPerkById(int anId) => allPerkDatas.FirstOrDefault(x => anId == x.itemId);

    void CreatePerk(PerkData data)
    {
        Perk perk = Instantiate(perkPrefab, perkContent).GetComponent<Perk>();
        allEquippedPerks.Add(perk);
        perk.BindData(data);
    }
    public void UnlockPerk(int id) => UnlockPerk(allPerkDatas.FirstOrDefault(x => x.itemId == id));

    public void UnlockPerk(PerkData data)
    {
        
        if (data == null) return;
        AddPerk(data);
        WorldSystem.instance.SaveProgression();
    }
    public void AddPerk(PerkData data)
    {
        List<PerkData> perks = allPerkDatas.Where(x => x.name == data.name).ToList();
        if (perks.FirstOrDefault(x => x.level == data.level - 1) is PerkData oldPerk && perks.FirstOrDefault(x => x.level == data.level) is PerkData newPerk)
        {
            if(allEquippedPerks.FirstOrDefault(x => x.perkData == oldPerk) is Perk aPerk)
            {
                allEquippedPerks.Remove(aPerk);
                aPerk?.DestroyPerk();
                CreatePerk(data);
            }
        }
        else
            CreatePerk(data);
    }
    public void UpdatePerks()
    {
        allEquippedPerks.ForEach(x => x.Activated = true);
    }
    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        allPerkDatas.Where(x => a_SaveData.unlockedPerks.Contains(x.itemId)).ToList().ForEach(x => allEquippedPerksDatas.Add(x));
    }
    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.unlockedPerks = allEquippedPerks.Select(x => x.perkData.itemId).ToList();
    }
    void Awake()
    {
        EquipmentSlot slot;
        for (int i = 0; i < inventorySlots; i++)
        {
            slot = Instantiate(itemSlotPrefab, stashContent).GetComponent<EquipmentSlot>();
            slot.name = string.Format("EquipmentSlot{0}", i);
            equipmentSlots.Add(slot);
        }

        for (int i = 0; i < gearSlots.Count; i++)
            gearSlots[i].gearImage.sprite = gearDisplays.First(x => x.equipmentType == gearSlots[i].equipmentType).artwork;
    }

    public EquipmentSlot RequestInventorySpace() => equipmentSlots.FirstOrDefault(x => x.Equipment == null);

    public void AssignInvetorySpace() => usedInventorySlots++;
    public bool AddEquipmentToInventory(Equipment equipment, EquipmentSlot optionalSpot = null)
    {
        if (equipmentSlots.Count < usedInventorySlots) 
        {
            WorldSystem.instance.uiManager.UIWarningController.CreateWarning("Not enough space in inventory!");
            return false;
        }
        if (optionalSpot != null)
        {
            optionalSpot.Equipment = equipment;
        }
        else
        {
            for (int i = 0; i < equipmentSlots.Count; i++)
            {
                if (equipmentSlots[i].Equipment == null)
                {
                    if (RequestInventorySpace() is EquipmentSlot slot) 
                    {
                        slot.Equipment = equipment;
                        AssignInvetorySpace();
                    }
                    break;
                }
            }
        }
        return true;
    }
}

[System.Serializable]
public struct GearDisplay
{
    public Sprite artwork;
    public EquipmentType equipmentType;
}