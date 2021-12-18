using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class EquipmentManager : Manager, ISaveableTemp
{
    public EquipmentSlot headSlot, chestSlot, handsSlot, legsSlot, feetSlots;
    public EquipmentSlotSmith smithHeadSlot, smithChestSlot, smithHandsSlot, smithLegsSlot, smithFeetSlots;
    public Dictionary<EquipmentType, EquipmentSlot> allEquipment;
    public EquipmentStruct equipmentStruct;
    public List<EquipmentData> equipmentDatas;
    public int maxUpgradeLevel = 5;
    protected override void Awake()
    {
        base.Awake();
        world.equipmentManager = this;
        allEquipment = new Dictionary<EquipmentType, EquipmentSlot>{
            {EquipmentType.Head, headSlot},
            {EquipmentType.Chest, chestSlot},
            {EquipmentType.Hands, handsSlot},
            {EquipmentType.Legs, legsSlot},
            {EquipmentType.Feet, feetSlots}
        };
    }
    protected override void Start()
    {
        base.Start();
    }
    public void UpgradeEquipment(EquipmentType equipmentType, bool increment = true)
    {
        EquipmentData data;
        switch (equipmentType)
        {
            case EquipmentType.Head:
                if (increment) equipmentStruct.headLevel++;
                data = equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.headLevel);
                headSlot.BindData(data, equipmentStruct.headLevel, true);
                smithHeadSlot.BindData(data, equipmentStruct.headLevel);
                break;
            case EquipmentType.Chest:
                if (increment) equipmentStruct.chestLevel++;
                data = equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.chestLevel);
                chestSlot.BindData(data, equipmentStruct.chestLevel, true);
                smithChestSlot.BindData(data, equipmentStruct.chestLevel);
                break;
            case EquipmentType.Hands:
                Debug.Log(equipmentStruct.handsLevel);
                if (increment) equipmentStruct.handsLevel++;
                data = equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.handsLevel);
                handsSlot.BindData(data, equipmentStruct.handsLevel, true);
                smithHandsSlot.BindData(data, equipmentStruct.handsLevel);
                break;
            case EquipmentType.Legs:
                if (increment) equipmentStruct.legsLevel++;
                data = equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.legsLevel);
                legsSlot.BindData(data, equipmentStruct.legsLevel, true);
                smithLegsSlot.BindData(data, equipmentStruct.legsLevel);
                break;
            case EquipmentType.Feet:
                if (increment) equipmentStruct.feetLevel++;
                data = equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.feetLevel);
                feetSlots.BindData(data, equipmentStruct.feetLevel, true);
                smithFeetSlots.BindData(data, equipmentStruct.feetLevel);
                break;
            default:
                break;
        }
    }
    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        equipmentStruct = a_SaveData.equipmentStruct;

        if (equipmentStruct == null)
        {
            equipmentStruct = new EquipmentStruct();
            UpgradeEquipment(EquipmentType.Head);
            UpgradeEquipment(EquipmentType.Chest);
            UpgradeEquipment(EquipmentType.Hands);
            UpgradeEquipment(EquipmentType.Legs);
            UpgradeEquipment(EquipmentType.Feet);
        }
        else
        {
            UpgradeEquipment(EquipmentType.Head, false);
            UpgradeEquipment(EquipmentType.Chest, false);
            UpgradeEquipment(EquipmentType.Hands, false);
            UpgradeEquipment(EquipmentType.Legs, false);
            UpgradeEquipment(EquipmentType.Feet, false);
        }
        
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.equipmentStruct = equipmentStruct;
    }
}

[System.Serializable] public class EquipmentStruct
{
    public int headLevel, chestLevel, handsLevel, legsLevel, feetLevel;
}
