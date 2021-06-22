using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectStrengthEOT : CardEffect
{
    public override bool isBuff { get { return true; } }

    public CardEffectStrengthEOT() : base()
    {
        OnNewTurn = null;
        OnEndTurn = _OnEndTurn;
    }

    public override void RespondStackUpdate(int update)
    {
       combatController.StartCoroutine(actor.RecieveEffectNonDamageNonBlock(new CardEffectInfo(EffectType.Strength, update, 1)));
    }

    
    public override IEnumerator _OnEndTurn()
    {
        yield return combatController.StartCoroutine(RecieveInput(-nrStacked));
    }
}
