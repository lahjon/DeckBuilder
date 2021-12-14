using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class StatHealth : Stat
{
    public StatHealth(bool aVisible)
    {
        type = StatType.Health;
        visible = aVisible;
    }

    public override void AddStatModifier(ItemEffectAddStat statModifer)
    {
        statModifers.Add(statModifer);
        HandleHealth(statModifer.GetValue());
        EventManager.StatModified(StatType.Health);
    }

    public override void RemoveStatModifier(ItemEffectAddStat statModifer)
    {
        statModifers.Remove(statModifer);
        HandleHealth(-statModifer.GetValue());
        EventManager.StatModified(StatType.Health);
    }

    public void HandleHealth(int val)
    {
        CharacterManager charMan = WorldSystem.instance.characterManager;
        value += val;
        if (value <= 1)
            value = 1;

        charMan.CurrentHealth += val;

        if (charMan.CurrentHealth < 1)
            charMan.CurrentHealth = 1;
    }
}