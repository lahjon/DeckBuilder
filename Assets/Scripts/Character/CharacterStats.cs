using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterStats : MonoBehaviour
{
    CharacterManager characterManager;
    [SerializeField][NonReorderable]
    public List<Stat> stats;
    public void Init()
    {
        characterManager = WorldSystem.instance.characterManager;
        AddStatsFromCharacter();
    }

    void Awake()
    {
        stats = new List<Stat>
                            {
                                new Stat(0, StatType.Health, true),
                                new Stat(0, StatType.Power, true), 
                                new Stat(0, StatType.Endurance, true), 
                                new Stat(0, StatType.Wit, true), 
                                new Stat(0, StatType.Amplitude, true), 
                                new Stat(0, StatType.Syphon, false), 
                                new Stat(0, StatType.DraftAmount, false),
                                new Stat(0, StatType.OptionalEnergyMax, false),
                                new Stat(0, StatType.OptionalEnergyTurn, false),
                                new Stat(0, StatType.MaxCardsOnHand, false)
                            };
    }

    void AddStatsFromCharacter()
    {
        characterManager.characterData.stats.ForEach(x => ModifyStat(x.statType, x.value, new IEffectAdderStruct("Character", x.value), true));
    }

    void RemoveStatsFromCharacter()
    {
        stats.ForEach(s => s.RemoveStatModifier(s.StatModifers.FirstOrDefault(x => x.GetName() == "Character"))); 
    }

    public int GetStatValue(StatType aStatType)
    {
        if (stats?.Any() == true && aStatType != StatType.None)
            return stats.Where(x => x.type == aStatType).FirstOrDefault().value;;
        return 0;
    }
    public Stat GetStat(StatType aStatType)
    {
        if (stats?.Any() == true)
            return stats.Where(x => x.type == aStatType).FirstOrDefault();
        return null;
    }

    public void ModifyHealth(int aValue, IEffectAdder effectAdder, bool addStat)
    {
        Stat stat = GetStat(StatType.Health);
        if (stat == null) return;
        stat.value += aValue;

        characterManager.currentHealth += aValue;

        if (GetStatValue(StatType.Health) < 1)
            stat.value = 1;

        if (characterManager.currentHealth < 1)
            characterManager.currentHealth = 1;

        if (addStat) stat.AddStatModifier(effectAdder);
        else stat.RemoveStatModifier(effectAdder);

        characterManager.characterVariablesUI.UpdateCharacterHUD();
    }

    public void ModifyStat(StatType aStatType, int aValue, IEffectAdder effectAdder, bool addStat)
    {
        if (aStatType == StatType.Health) 
        {
            ModifyHealth(aValue, effectAdder, addStat);
            return;
        }
        Stat stat = GetStat(aStatType);
        if (stat == null) return;
        Debug.Log(aStatType);
        if (addStat) stat.AddStatModifier(effectAdder);
        else stat.RemoveStatModifier(effectAdder);

        stat.value += aValue;
        characterManager.characterVariablesUI.UpdateCharacterHUD();
    }
}

public interface IStatsAdder
{
    public int GetValue();
    public string GetSource();
}

[System.Serializable]
public struct StatStruct
{
    public StatStruct(int aValue, StatType aStatType)
    {
        statType = aStatType;
        value = aValue;
    }
    public int value;
    public StatType statType;
}