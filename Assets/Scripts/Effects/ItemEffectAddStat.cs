using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddStat : ItemEffect
{
    public override void AddItemEffect()
    {
        WorldSystem.instance.characterManager.characterStats.ModifyHealth(7);
    }
    public override void RemoveItemEffect()
    {
        WorldSystem.instance.characterManager.characterStats.ModifyHealth(-7);
    }
}
