using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCombatEffect : ItemEffect
{
    public override void AddItemEffect()
    {
        if (itemEffectStruct.instant)
            CombatSystem.instance.StartCoroutine(CombatEffectAdd());
        else
            CombatSystem.instance.Hero.actionsStartCombat.Add(CombatEffectAdd);
    }
    public override void RemoveItemEffect()
    {
        if (itemEffectStruct.instant)
            CombatSystem.instance.StartCoroutine(CombatEffectRemove());
        else
            CombatSystem.instance.Hero.actionsStartCombat.Add(CombatEffectRemove);
    }
    IEnumerator CombatEffectAdd()
    {
        CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(itemEffectStruct.parameter.ToEnum<EffectType>(), itemEffectStruct.value)));
        yield return null;
    }

    IEnumerator CombatEffectRemove()
    {
        CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(itemEffectStruct.parameter.ToEnum<EffectType>(), -itemEffectStruct.value)));
        yield return null;
    }
}


