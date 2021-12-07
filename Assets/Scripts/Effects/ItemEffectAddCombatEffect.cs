using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCombatEffect : ItemEffect
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        if (itemEffectStruct.addImmediately)
            CombatSystem.instance.StartCoroutine(RunEffectEnumerator());
    }

    public override IEnumerator RunEffectEnumerator()
    {
        effectAdder.NotifyUsed();
        CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(itemEffectStruct.parameter.ToEnum<EffectType>(), itemEffectStruct.value)));
        yield return null;
    }

    public override void Register()
    {
        base.Register();
        if (itemEffectStruct.addImmediately)
            ApplyEffect();
        else
            CombatSystem.instance.effectOnCombatStart.Add(this);
    }

    public override void DeRegister()
    {
        base.DeRegister();
        if (!itemEffectStruct.addImmediately)
            CombatSystem.instance.effectOnCombatStart.Remove(this);

    }
}
