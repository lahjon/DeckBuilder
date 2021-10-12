using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MenuCharacter : MonoBehaviour, ISaveableWorld
{
    public List<PerkData> allPerkDatas = new List<PerkData>();
    public List<PerkData> allEquippedPerksDatas = new List<PerkData>();
    public List<Perk> allEquippedPerks;
    public Transform perkContent;
    public GameObject perkPrefab;

    void Start()
    {
        allEquippedPerksDatas.ForEach(x => AddPerk(x));
        allEquippedPerksDatas.Clear();
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
