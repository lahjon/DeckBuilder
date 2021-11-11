using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCombatStat : ItemEffect
{
    public override void AddItemEffect()
    {
        EffectSwitch(itemEffectStruct.parameter.ToEnum<EffectType>(), itemEffectStruct.value);
    }
    public override void RemoveItemEffect()
    {
        EffectSwitch(itemEffectStruct.parameter.ToEnum<EffectType>(), -itemEffectStruct.value);
    }
    void EffectSwitch(EffectType effect, int amount)
    {
        switch (effect)
        {
            case EffectType.Strength:
                CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(EffectType.Strength, amount)));
                break;
            case EffectType.StrengthTemp:
                CombatSystem.instance.StartCoroutine(CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(EffectType.StrengthTemp, amount)));
                break;
            default:
                Debug.LogWarning(string.Format("No case implemented for {0}!", effect.ToString()));
                break;
        }
    }
}
