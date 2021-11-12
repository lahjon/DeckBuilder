using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class MenuCharacter : MonoBehaviour, ISaveableWorld
{
    public List<PerkData> allPerkDatas = new List<PerkData>();
    public List<PerkData> allEquippedPerksDatas = new List<PerkData>();
    public List<Perk> allEquippedPerks = new List<Perk>();
    public Transform perkContent;
    public GameObject perkPrefab;
    public GameObject tooltipPanel;
    public TMP_Text tooltipName;
    public TMP_Text tooltipDescription;

    public void Init()
    {
        DeactivateToolTip();
        allEquippedPerksDatas.ForEach(x => CreatePerk(x));
        allEquippedPerksDatas.Clear();
    }
    public void ActivateToolTip(EquipmentData data)
    {
        if (data == null) return;
        tooltipPanel.SetActive(true);
        tooltipName.text = data.itemName;
        tooltipDescription.text = data.description;
    }
    public void ActivateToolTip(PerkData data)
    {
        if (data == null) return;
        tooltipPanel.SetActive(true);
        tooltipName.text = data.itemName;
        tooltipDescription.text = data.description;
    }
    
    public void DeactivateToolTip()
    {
        tooltipPanel.SetActive(false);
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
}
