using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEffectThorns : Effect
{
    CombatController combatController;
    public override void AddEffect()
    {
        combatController = WorldSystem.instance.combatManager.combatController;
        Debug.Log(string.Format("Adding effect {0}!", this.GetType().Name));
        combatController.Hero.actionsStartCombat.Add(ApplyThorns);
    }

    public override void RemoveEffect()
    {
        Debug.Log(string.Format("Removing effect {0}!", this.GetType().Name));
        combatController.Hero.actionsStartCombat.Remove(ApplyThorns);
    }

    IEnumerator ApplyThorns()
    {

        combatController.Hero.RecieveEffectNonDamageNonBlock(new CardEffect() { Type = EffectType.Thorns, Times = 1, Value = 3 });
        yield return null;
    }
}