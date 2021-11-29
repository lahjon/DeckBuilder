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
        EventManager.StatModified();
        HandleHealth(statModifer.GetValue());
    }

    public override void RemoveStatModifier(ItemEffectAddStat statModifer)
    {
        statModifers.Remove(statModifer);
        EventManager.StatModified();
        HandleHealth(-statModifer.GetValue());
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