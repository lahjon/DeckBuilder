using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddStat : ItemEffect
{
    public int GetValue() => itemEffectStruct.value;
    public string GetName() => effectAdder.GetName();

    public override void AddItemEffect()
    {
        CharacterStats.stats[itemEffectStruct.parameter.ToEnum<StatType>()].AddStatModifier(this);
    }
    public override void RemoveItemEffect()
    {
        CharacterStats.stats[itemEffectStruct.parameter.ToEnum<StatType>()].RemoveStatModifier(this);
    }

}
