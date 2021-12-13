using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectStrengthTemp : StatusEffect
{

    public StatusEffectStrengthTemp() : base()
    {
        OnNewTurn = null;
        OnEndTurn = _OnEndTurn;
    }

    public override void RespondStackUpdate(int update)
    {
       CombatSystem.instance.StartCoroutine(actor.RecieveEffectNonDamageNonBlock(new StatusEffectCarrier(StatusEffectType.Strength, update, 1)));
    }

    
    protected override IEnumerator _OnEndTurn()
    {
        yield return CombatSystem.instance.StartCoroutine(RecieveInput(-nrStacked));
    }
}
