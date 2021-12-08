using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCombatActivity : ItemEffect
{
    CardActivitySetting CardActivity;

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        if (WorldSystem.instance.worldState == WorldState.Combat)
            CombatSystem.instance.queuedEffects.Enqueue(this);
    }

    public override IEnumerator RunEffectEnumerator()
    {
        effectAdder.NotifyUsed();

        yield return CombatSystem.instance.StartCoroutine(CardActivitySystem.instance.StartByCardActivity(CardActivity));
    }

    public override void Register()
    {
        base.Register();
        string[] settings = itemEffectStruct.parameter.Split('|');
        CardActivity = new CardActivitySetting(new CardActivityData() { type = settings[0].ToEnum<CardActivityType>(), strParameter = settings[1], val = itemEffectStruct.value });

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
