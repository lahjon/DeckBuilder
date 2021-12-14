using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddStat : ItemEffect
{
    public int GetValue() => itemEffectStruct.value;
    public string GetName() => effectAdder.GetName();

    public override void Register()
    {
        base.Register();
        Debug.Log("Register New Addstat Effect");
        CharacterStats.stats[itemEffectStruct.parameter.ToEnum<StatType>()].AddStatModifier(this);
    }

    public override void DeRegister()
    {
        base.DeRegister();
        CharacterStats.stats[itemEffectStruct.parameter.ToEnum<StatType>()].RemoveStatModifier(this);
    }



}
