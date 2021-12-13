using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityManager : Manager
{
    public AbilitySlot minorSlot;
    public AbilitySlot majorSlot;
    public List<AbilityData> allAbilities { get => DatabaseSystem.instance.abilityDatas; }
    public List<AbilityData> unlockedAbilities = new List<AbilityData>(); 
    protected override void Awake()
    {
        base.Awake();
        world.abilityManager = this;
    }
    protected override void Start()
    {
        base.Start();
    }

    public AbilityData GetAbilityDataById(int anId = -1)
    {
        if (anId >= 0)
            return allAbilities.Except(new List<AbilityData>{minorSlot.ability.abilityData, majorSlot.ability.abilityData}).FirstOrDefault(x => x.itemId == anId);
        else
        {
            AbilityData[] items = allAbilities.Except(new List<AbilityData>{minorSlot.ability.abilityData, majorSlot.ability.abilityData}).ToArray();
            if (items.Count() > 0)
                return items[Random.Range(0, items.Count())];

            return null;
        }
    }
    public void UnlockAbility(int anId = -1)
    {
        if (anId >= 0 && allAbilities.Except(new List<AbilityData>{minorSlot.ability.abilityData, majorSlot.ability.abilityData}).FirstOrDefault(x => x.itemId == anId) is AbilityData data)
        {
            unlockedAbilities.Add(data);
            WorldSystem.instance.SaveProgression();
        }
    }

    public void EquipAbility(int anId = -1)
    {
        AbilityData data;

        if (anId >= 0)
            data = allAbilities.Except(new List<AbilityData>{minorSlot.ability.abilityData, majorSlot.ability.abilityData}).FirstOrDefault(x => x.itemId == anId);
        else
            data = GetAbilityDataById();

        if (data == null)
            return;

        if (data.abilityType == AbilityType.Major)
            majorSlot.ability.BindData(data);
        else
            minorSlot.ability.BindData(data);
    }

    // public void PopulateSaveDataTemp(SaveDataTemp a_SaveData)
    // {
    //     a_SaveData.selectedUseItems = equippedItems.Select(x => x.itemData.itemId).ToList();
    // }

    // public void LoadFromSaveDataTemp(SaveDataTemp a_SaveData)
    // {
    //     a_SaveData.selectedUseItems.ForEach(x => AddItem(x));
    // }
}
