using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectHeal : ItemEffect
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        WorldSystem.instance.characterManager.Heal(itemEffectStruct.value);
    }

    public override void Register()
    {
        if (itemEffectStruct.addImmediately)
            ApplyEffect();
    }
}
