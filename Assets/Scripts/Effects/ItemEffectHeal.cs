using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectHeal : ItemEffect
{
    public override void AddItemEffect()
    {
        WorldSystem.instance.characterManager.Heal(itemEffectStruct.value);
    }
    public override void RemoveItemEffect()
    {
        WorldSystem.instance.characterManager.Heal(-itemEffectStruct.value);
    }
}
