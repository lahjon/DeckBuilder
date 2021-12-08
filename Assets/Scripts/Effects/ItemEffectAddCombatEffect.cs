using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCombatEffect : ItemEffect
{
    public override void ApplyEffect()
    {
        base.ApplyEffect();
        if (WorldSystem.instance.worldState == WorldState.Combat)
            CombatSystem.instance.queuedEffects.Enqueue(this);
    }

    public override IEnumerator RunEffectEnumerator()
    {
        effectAdder.NotifyUsed();
        yield return CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(itemEffectStruct.parameter.ToEnum<EffectType>(), itemEffectStruct.value)));
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
