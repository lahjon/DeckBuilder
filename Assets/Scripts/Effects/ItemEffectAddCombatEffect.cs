using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCombatEffect : ItemEffect
{
    StatusEffectCarrier effect;
    StatusEffectCarrier effectReversed;

    Queue<StatusEffectCarrier> effectQueue = new Queue<StatusEffectCarrier>();

    public override IEnumerator RunEffectEnumerator()
    {
        effectAdder.NotifyUsed();
        yield return CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(effectQueue.Dequeue()));
    }

    public override void Register()
    {
        base.Register();
        if(effect == null) effect = new StatusEffectCarrier(itemEffectStruct.parameter.ToEnum<StatusEffectType>(), itemEffectStruct.value);
        if (effectReversed == null) effectReversed = new StatusEffectCarrier(itemEffectStruct.parameter.ToEnum<StatusEffectType>(), -itemEffectStruct.value);
        if (!itemEffectStruct.addImmediately)
            CombatSystem.instance.effectOnCombatStart.Add(this);

        if (WorldStateSystem.instance.currentWorldState == WorldState.Combat)
        {
            effectQueue.Enqueue(effect);
            CombatSystem.instance.QueueEffect(this);
        }
    }

    public override void DeRegister()
    {
        base.DeRegister();
        if (!itemEffectStruct.addImmediately)
            CombatSystem.instance.effectOnCombatStart.Remove(this);

        if (WorldStateSystem.instance.currentWorldState == WorldState.Combat)
        {
            effectQueue.Enqueue(effectReversed);
            CombatSystem.instance.QueueEffect(this);
        }
    }
}
