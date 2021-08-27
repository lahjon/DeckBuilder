using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSuperSword : Effect
{
    public override void AddEffect()
    {
        Debug.Log(string.Format("Adding effect {0}!", this.GetType().Name));
        CombatSystem.instance.StartCoroutine(
            CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectCarrier() { Type = EffectType.Strength, Times = 1, Value = 3 }));
    }

    public override void RemoveEffect()
    {
        Debug.Log(string.Format("Removing effect {0}!", this.GetType().Name));
    }

  
}
