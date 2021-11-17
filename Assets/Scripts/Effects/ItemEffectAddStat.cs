using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddStat : ItemEffect
{
    public override void AddItemEffect()
    {
        base.AddItemEffect();
        StatSwitch(itemEffectStruct.parameter.ToEnum<StatType>(), itemEffectStruct.value);
    }
    public override void RemoveItemEffect()
    {
        base.RemoveItemEffect();
        StatSwitch(itemEffectStruct.parameter.ToEnum<StatType>(), -itemEffectStruct.value);
    }
    void StatSwitch(StatType statType, int amount)
    {
        if (statType == StatType.Health)
            WorldSystem.instance.characterManager.characterStats.ModifyHealth(amount);
        else
            WorldSystem.instance.characterManager.characterStats.ModifyStat(statType, amount);
    }
}
