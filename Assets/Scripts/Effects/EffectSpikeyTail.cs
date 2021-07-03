using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSpikeyTail : Effect
{
    public override void AddEffect()
    {
        Debug.Log(string.Format("Adding effect {0}!", this.GetType().Name));
        CombatSystem.instance.Hero.actionsStartCombat.Add(ApplyThorns);
    }

    public override void RemoveEffect()
    {
        Debug.Log(string.Format("Removing effect {0}!", this.GetType().Name));
        CombatSystem.instance.Hero.actionsStartCombat.Remove(ApplyThorns);
    }

    IEnumerator ApplyThorns()
    {
        CombatSystem.instance.Hero.RecieveEffectNonDamageNonBlock(new CardEffectInfo() { Type = EffectType.Thorns, Times = 1, Value = 3 });
        yield return null;
    }
}
