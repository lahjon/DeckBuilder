using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public enum StatType
{
    None = 0,

    // main stats
    Health,                     // HP
    Power,                      // damage to attacks
    Wit,                        // cards to draw each turn
    Amplitude,                  // energy max
    Syphon,                     // energy / turn
    Endurance,                  // armor / block ??

    // hidden stats
    OptionalEnergyMax = 10,
    OptionalEnergyTurn,
    DraftAmount,
    MaxCardsOnHand

}

public class CharacterStats : MonoBehaviour
{
    CharacterManager characterManager;

    public static Stat Health = new StatHealth(true);
    public static Stat Power = new Stat(StatType.Power, true);
    public static Stat Endurance = new Stat(StatType.Endurance, true);
    public static Stat Wit = new Stat(StatType.Wit, true);
    public static Stat Amplitude = new Stat(StatType.Amplitude, true);
    public static Stat Syphon = new Stat(StatType.Syphon, false);
    public static Stat DraftAmount = new Stat(StatType.DraftAmount, false);
    public static Stat OptionalEnergyMax = new Stat(StatType.OptionalEnergyMax, false);
    public static Stat OptionalEnergyTurn = new Stat(StatType.OptionalEnergyTurn, false);
    public static Stat MaxCardsOnHand = new Stat(StatType.MaxCardsOnHand, false);

    public static Dictionary<StatType, Stat> stats = new Dictionary<StatType, Stat> { 
        { StatType.Health, Health }, 
        { StatType.Power, Power }, 
        { StatType.Endurance, Endurance}, 
        { StatType.Wit, Wit}, 
        { StatType.Amplitude, Amplitude}, 
        { StatType.Syphon, Syphon}, 
        { StatType.DraftAmount,DraftAmount}, 
        { StatType.OptionalEnergyMax, OptionalEnergyMax}, 
        { StatType.OptionalEnergyTurn, OptionalEnergyTurn}, 
        { StatType.MaxCardsOnHand, MaxCardsOnHand}, 
    };

    public void Init()
    {
        characterManager = WorldSystem.instance.characterManager;
        AddStatsFromCharacter();
    }

    void AddStatsFromCharacter()
    {
        characterManager.characterData.stats.ForEach(x =>
        {
            if (x.statType == StatType.None) return;
            ItemEffectAddStat effect = new ItemEffectAddStat();
            effect.effectAdder = characterManager;
            effect.world = WorldSystem.instance;
            effect.itemEffectStruct = new ItemEffectStruct(ItemEffectType.AddStat, x.statType.ToString(), x.value,true);
            stats[x.statType].AddStatModifier(effect);
        }
        );
    }

    void RemoveStatsFromCharacter()
    {
        stats.Values.ToList().ForEach(s => s.RemoveModifierFromOwner(characterManager)); 
    }
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
