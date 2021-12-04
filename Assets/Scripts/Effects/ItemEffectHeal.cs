using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectHeal : ItemEffect
{
    public override void ApplyEffect()
    {
        Debug.Log("applying");
        base.ApplyEffect();
        WorldSystem.instance.characterManager.Heal(itemEffectStruct.value);
    }

    public override void Register()
    {
        Debug.Log("registering");
        if (itemEffectStruct.addImmediately)
            ApplyEffect();
    }
}
