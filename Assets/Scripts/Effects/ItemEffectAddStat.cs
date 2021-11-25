using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddStat : ItemEffect
{
    public override void AddItemEffect()
    {
        base.AddItemEffect();
        StatSwitch(itemEffectStruct.parameter.ToEnum<StatType>(), itemEffectStruct.value, true);
    }
    public override void RemoveItemEffect()
    {
        base.RemoveItemEffect();
        StatSwitch(itemEffectStruct.parameter.ToEnum<StatType>(), -itemEffectStruct.value, false);
    }
    void StatSwitch(StatType statType, int amount, bool add)
    {
        if (statType == StatType.Health)
            WorldSystem.instance.characterManager.characterStats.ModifyHealth(amount, effectAdder, add);
        else
            WorldSystem.instance.characterManager.characterStats.ModifyStat(statType, amount, effectAdder, add);
    }
}
