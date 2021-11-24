using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCombatEffect : ItemEffect, IEffect
{
    public override void AddItemEffect()
    {
        base.AddItemEffect();
        if (!itemEffectStruct.addOnStart)
            CombatSystem.instance.StartCoroutine(TriggerEffect());
        else
            CombatSystem.instance.effectOnCombatStart.Add(this);
    }
    public override void RemoveItemEffect()
    {
        base.RemoveItemEffect();
        if (!itemEffectStruct.addOnStart)
            CombatSystem.instance.StartCoroutine(RemoveEffect());
        else
            CombatSystem.instance.effectOnCombatStart.Remove(this);
    }
    public IEnumerator TriggerEffect()
    {
        CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(itemEffectStruct.parameter.ToEnum<EffectType>(), itemEffectStruct.value)));
        yield return null;
    }

    public IEnumerator RemoveEffect()
    {
        CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(itemEffectStruct.parameter.ToEnum<EffectType>(), -itemEffectStruct.value)));
        yield return null;
    }
    public IEffectAdder EffectAdder() => effectAdder;
}
