using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class EquipmentManager : Manager
{
    public GameObject equipmentPrefab;
    public Transform itemsParent;
    protected override void Awake()
    {
        base.Awake();
        world.equipmentManager = this;
    }
    protected override void Start()
    {
        base.Start();
    }

    public Equipment CreateEquipment(Transform parent = null)
    {
        if (parent == null) parent = itemsParent;
        Equipment newEquipment = Instantiate(equipmentPrefab, parent).GetComponent<Equipment>();
        newEquipment.transform.localScale = Vector3.one;
        EquipmentFilter ef = new EquipmentFilter(){

        };
        
        EquipmentDataStruct equipmentDataStruct = RandomGenerator.GenerateEquipmentDataStruct(ef);
        newEquipment.BindData(equipmentDataStruct);
        return newEquipment;
    }
}

public static class RandomGenerator
{
    static Dictionary<Rarity, int> RarityLookup = new Dictionary<Rarity, int>()
    {
        {Rarity.None, 0},
        {Rarity.Common, 50},
        {Rarity.Uncommon, 35},
        {Rarity.Rare, 15},
        {Rarity.Epic, 5}
    };
    static List<EquipmentType> equipmentTypes = new List<EquipmentType>();

    static RandomGenerator()
    {
        equipmentTypes = System.Enum.GetValues(typeof(EquipmentType)).Cast<EquipmentType>().ToList();
        equipmentTypes.Remove(EquipmentType.None);
    }

    public static EquipmentDataStruct GenerateEquipmentDataStruct(EquipmentFilter ef)
    {
        
        Rarity rarity = ef.rarity == Rarity.None ? GetRandomRarity() : ef.rarity;
        EquipmentType equipmentType = ef.equipmentType == EquipmentType.None ? GetRandomEquipmentType() : ef.equipmentType;
        int level = ef.level <= 0 ? GenerateRandomLevel(WorldSystem.instance.characterManager.Level - 1, WorldSystem.instance.characterManager.Level + 2) : ef.level;
        List<ItemEffectStruct> itemEffectStructs = null;

        return new EquipmentDataStruct(level, equipmentType, rarity, itemEffectStructs);
    }

    public static void SetSeed(int seedNumber) => Random.InitState(seedNumber);
    public static Rarity GetRandomRarity(int rarityAdd = 0)
    {
        int random = Random.Range(0, 101);
        Debug.Log(random);
        if (random >= RarityLookup[Rarity.Uncommon] + rarityAdd) return Rarity.Common;
        else if (random >= RarityLookup[Rarity.Rare] + rarityAdd) return Rarity.Uncommon;
        else if (random >= RarityLookup[Rarity.Epic] + rarityAdd) return Rarity.Rare;
        else return Rarity.Epic;
    }

    public static EquipmentType GetRandomEquipmentType() => equipmentTypes[Random.Range(0, equipmentTypes.Count)];
    public static int GenerateRandomLevel(int min, int maxExclusive) => Random.Range(min, maxExclusive);
}
[System.Serializable]
public struct EquipmentDataStruct
{
    public int level;
    public EquipmentType equipmentType;
    public Rarity rarity;
    public List<ItemEffectStruct> itemEffectStructs;
    public EquipmentDataStruct(int aLevel, EquipmentType aEquipmentType, Rarity aRarity, List<ItemEffectStruct> aItemEffectStructs)
    {
        level = aLevel;
        equipmentType = aEquipmentType;
        rarity = aRarity;
        itemEffectStructs = aItemEffectStructs;
    }
}


public class EquipmentFilter
{
    #nullable enable
    public EquipmentType equipmentType;
    public int minLevel;
    public int maxLevel;
    public Rarity rarity;
    public int level;
    public int seed;

}