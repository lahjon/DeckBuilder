using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectTakeDamage : ItemEffect
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        //WorldSystem.instance.characterManager.Heal(itemEffectStruct.value);
    }
}
