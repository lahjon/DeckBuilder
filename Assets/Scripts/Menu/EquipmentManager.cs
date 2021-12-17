using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class EquipmentManager : Manager, ISaveableTemp
{
    public EquipmentSlot headSlot, chestSlot, handsSlot, legsSlot, feetSlots;
    public EquipmentStruct equipmentStruct;
    public List<EquipmentData> equipmentDatas;
    protected override void Awake()
    {
        base.Awake();
        world.equipmentManager = this;
    }
    protected override void Start()
    {
        base.Start();
    }
    public void UpgradeEquipment(EquipmentType equipmentType, bool increment = true)
    {
        switch (equipmentType)
        {
            case EquipmentType.Head:
                if (increment) equipmentStruct.headLevel++;
                headSlot.BindData(equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.headLevel));
                break;
            case EquipmentType.Chest:
                if (increment) equipmentStruct.chestLevel++;
                chestSlot.BindData(equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.chestLevel));
                break;
            case EquipmentType.Hands:
                if (increment) equipmentStruct.handsLevel++;
                handsSlot.BindData(equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.handsLevel));
                break;
            case EquipmentType.Legs:
                if (increment) equipmentStruct.legsLevel++;
                legsSlot.BindData(equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.legsLevel));
                break;
            case EquipmentType.Feet:
                if (increment) equipmentStruct.feetLevel++;
                feetSlots.BindData(equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.feetLevel));
                break;
            default:
                break;
        }
    }
    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        equipmentStruct = a_SaveData.equipmentStruct = equipmentStruct;

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

public class EquipmentStruct
{
    public int headLevel, chestLevel, handsLevel, legsLevel, feetLevel;
}
