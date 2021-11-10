using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffectAddCombatStat : ItemEffect
{
    public override void AddItemEffect()
    {
        //itemEffectStruct.type;
        //StatSwitch();
    }
    public override void RemoveItemEffect()
    {
        WorldSystem.instance.characterManager.characterStats.ModifyHealth(-7);
    }
    void StatSwitch(EffectType effect, int amount)
    {
        switch (EffectType.Strength)
        {
            case EffectType.Strength:
                CombatSystem.instance.StartCoroutine(
                CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier(EffectType.Strength, amount)));
                break;
            default:
                break;
        }
    }
}
