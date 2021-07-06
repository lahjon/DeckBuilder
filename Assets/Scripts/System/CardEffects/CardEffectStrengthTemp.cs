using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectStrengthTemp : CardEffect
{
    public override bool isBuff { get { return true; } }

    public CardEffectStrengthTemp() : base()
    {
        OnNewTurn = null;
        OnEndTurn = _OnEndTurn;
    }

    public override void RespondStackUpdate(int update)
    {
       CombatSystem.instance.StartCoroutine(actor.RecieveEffectNonDamageNonBlock(new CardEffectInfo(EffectType.Strength, update, 1)));
    }

    
    public override IEnumerator _OnEndTurn()
    {
        yield return CombatSystem.instance.StartCoroutine(RecieveInput(-nrStacked));
    }
}