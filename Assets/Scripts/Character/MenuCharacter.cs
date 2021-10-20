using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class MenuCharacter : MonoBehaviour, ISaveableWorld
{
    public List<PerkData> allPerkDatas = new List<PerkData>();
    public List<PerkData> allEquippedPerksDatas = new List<PerkData>();
    public List<Perk> allEquippedPerks;
    public Transform perkContent;
    public GameObject perkPrefab;
    public GameObject tooltipPanel;
    public TMP_Text tooltipName;
    public TMP_Text tooltipDescription;

    void Start()
    {
        allEquippedPerksDatas.ForEach(x => AddPerk(x));
        allEquippedPerksDatas.Clear();
        DeactivateToolTip();
    }
    public void ActivateToolTip(EquipmentData data)
    {
        if (data == null) return;
        tooltipPanel.SetActive(true);
        tooltipName.text = data.equipmentName;
        tooltipDescription.text = data.description;
    }
    public void DeactivateToolTip()
    {
        tooltipPanel.SetActive(false);
    }

    void AddPerk(PerkData data)
    {
        Perk perk = Instantiate(perkPrefab, perkContent).GetComponent<Perk>();
        allEquippedPerks.Add(perk);
        perk.BindData(data);
    }
    public void UnlockPerk(PerkData data)
    {
        AddPerk(data);
        WorldSystem.instance.SaveProgression();
    }
    public void UpdatePerks()
    {
        allEquippedPerks.ForEach(x => x.Activated = true);
    }
    public void LoadFromSaveDataWorld(SaveDataWorld a_SaveData)
    {
        allPerkDatas.Where(x => a_SaveData.unlockedPerks.Contains(x.perkId)).ToList().ForEach(x => allEquippedPerksDatas.Add(x));
    }
    public void PopulateSaveDataWorld(SaveDataWorld a_SaveData)
    {
        a_SaveData.unlockedPerks = allEquippedPerks.Select(x => x.perkData.perkId).ToList();
    }
}
