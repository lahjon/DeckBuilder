using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCombatEffect : ItemEffect
{
    CardEffectCarrier effect;
    CardEffectCarrier effectReversed;

    Queue<CardEffectCarrier> effectQueue = new Queue<CardEffectCarrier>();

    public override IEnumerator RunEffectEnumerator()
    {
        effectAdder.NotifyUsed();
        yield return CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(effectQueue.Dequeue()));
    }

    public override void Register()
    {
        //base.Register(); No, we want to trigger icon when effect is resolved;
        if(effect == null) effect = new CardEffectCarrier(itemEffectStruct.parameter.ToEnum<EffectType>(), itemEffectStruct.value);
        if (effect == null) effectReversed = new CardEffectCarrier(itemEffectStruct.parameter.ToEnum<EffectType>(), -itemEffectStruct.value);
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
