using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddStat : ItemEffect
{
    public override void AddItemEffect()
    {
        StatSwitch(itemEffectStruct.parameter.ToEnum<StatType>(), itemEffectStruct.value);
    }
    public override void RemoveItemEffect()
    {
        StatSwitch(itemEffectStruct.parameter.ToEnum<StatType>(), -itemEffectStruct.value);
    }
    void StatSwitch(StatType statType, int amount)
    {
        switch (statType)
        {
            case StatType.Health:
                WorldSystem.instance.characterManager.characterStats.ModifyHealth(amount);
                break;
            case StatType.Endurance:
                WorldSystem.instance.characterManager.characterStats.ModifyStat(statType, amount);
                break;
            default:
                Debug.LogWarning(string.Format("No case implemented for {0}!", statType.ToString()));
                break;
        }
    }
}
