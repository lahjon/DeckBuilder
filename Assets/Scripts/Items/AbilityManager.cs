using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AbilityManager : Manager
{
    public GameObject abilityPrefab;
    public Canvas canvas;
    public Transform content;
    public List<AbilityData> allAbilities { get => DatabaseSystem.instance.abilityDatas; }
    public List<Ability> currentAbilities = new List<Ability>(); 
    public int maxAbilitySlots;
    public int usedAbilitySlots;
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
            return allAbilities.Except(currentAbilities.Select(x => x.abilityData)).FirstOrDefault(x => x.itemId == anId);
        else
        {
            AbilityData[] items = allAbilities.Except(currentAbilities.Select(x => x.abilityData)).ToArray();
            if (items.Count() > 0)
                return items[Random.Range(0, items.Count())];

            return null;
        }
    }

    public void RemoveAbility(Ability ability)
    {
        if (currentAbilities.Contains(ability))
        {
            currentAbilities.Remove(ability);
            ability.RemoveAbility();
        }
    }

    public void RemoveAbility()
    {
        Ability ability = currentAbilities[Random.Range(0, currentAbilities.Count - 1)];
        if (ability != null)
        {
            currentAbilities.Remove(ability);
            ability.RemoveAbility();
        }
    }

    public void AddAbility(int anId = -1)
    {
        if (usedAbilitySlots >= maxAbilitySlots)
        {
            Debug.LogWarning("No free item slots!");
            return;
        }

        AbilityData data;

        if (anId >= 0)
            data = allAbilities.Except(currentAbilities.Select(x => x.abilityData)).FirstOrDefault(x => x.itemId == anId);
        else
            data = GetAbilityDataById();

        if (data == null)
            return;

        usedAbilitySlots++;
        Ability newAbility = Instantiate(abilityPrefab, content).GetComponent<Ability>();
        currentAbilities.Add(newAbility);
        newAbility.abilityData = data;
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
