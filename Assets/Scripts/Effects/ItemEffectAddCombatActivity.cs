using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCombatActivity : ItemEffect
{
    CardActivitySetting CardActivity;

    public override void ApplyEffect()
    {
        base.ApplyEffect();
        if (WorldStateSystem.instance.currentWorldState == WorldState.Combat)
            CombatSystem.instance.QueueEffect(this);
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
        CardActivity = new CardActivitySetting(new CardActivityData() { type = settings[0].ToEnum<CardActivityType>(), strParameter = settings.Length > 1 ? settings[1] : string.Empty, val = itemEffectStruct.value });

        if (!itemEffectStruct.addImmediately)
            CombatSystem.instance.effectOnCombatStart.Add(this);
    }

    public override void DeRegister()
    {
        base.DeRegister();
        if (!itemEffectStruct.addImmediately)
            CombatSystem.instance.effectOnCombatStart.Remove(this);

    }
}
