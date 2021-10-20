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
    public void UpgradeEquipment(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Head:
                equipmentStruct.headLevel++;
                headSlot.BindData(equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.headLevel));
                break;
            case EquipmentType.Chest:
                equipmentStruct.chestLevel++;
                chestSlot.BindData(equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.chestLevel));
                break;
            case EquipmentType.Hands:
                equipmentStruct.handsLevel++;
                handsSlot.BindData(equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.handsLevel));
                break;
            case EquipmentType.Legs:
                equipmentStruct.legsLevel++;
                legsSlot.BindData(equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.legsLevel));
                break;
            case EquipmentType.Feet:
                equipmentStruct.feetLevel++;
                feetSlots.BindData(equipmentDatas.Where(x => x.classType == world.characterManager.selectedCharacterClassType && x.equipmentType == equipmentType).FirstOrDefault(x => x.level == equipmentStruct.feetLevel));
                break;
            default:
                break;
        }
    }
    public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    {
        a_SaveData.equipmentStruct = equipmentStruct;

        if (equipmentStruct == null)
        {
            equipmentStruct = new EquipmentStruct();
            UpgradeEquipment(EquipmentType.Head);
            UpgradeEquipment(EquipmentType.Chest);
            UpgradeEquipment(EquipmentType.Hands);
            UpgradeEquipment(EquipmentType.Legs);
            UpgradeEquipment(EquipmentType.Feet);
        }
    }

    public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    {
        equipmentStruct = a_SaveData.equipmentStruct;
    }
}

// public class EquipmentCharacter
// {
//     public CharacterClassType classType;
//     public List<EquipmentStruct> equipmentStructs;
//     public EquipmentCharacter()
//     {
//         equipmentStructs = new List<EquipmentStruct>();
//         equipmentStructs.Add(new EquipmentStruct(EquipmentType.Head, 1));
//         equipmentStructs.Add(new EquipmentStruct(EquipmentType.Chest, 1));
//         equipmentStructs.Add(new EquipmentStruct(EquipmentType.Hands, 1));
//         equipmentStructs.Add(new EquipmentStruct(EquipmentType.Legs, 1));
//         equipmentStructs.Add(new EquipmentStruct(EquipmentType.Feet, 1));
//     }
// }

// public class EquipmentStruct
// {
//     public EquipmentType equipmentType;
//     public int level;
//     public EquipmentStruct(EquipmentType anEquipmentType, int aLevel)
//     {
//         equipmentType = anEquipmentType;
//         level = aLevel;
//     }
// }

public class EquipmentStruct
{
    public int headLevel, chestLevel, handsLevel, legsLevel, feetLevel;
}
