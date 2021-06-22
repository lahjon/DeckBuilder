using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectPoison : CardEffect
{
    public override bool isBuff { get { return false; } }

    private float poisonAnimationTime = 0.7f;

    public CardEffectPoison() : base()
    {
        OnNewTurn = null;
    }

    public override void AddFunctionToRules()
    {
        actor.actionsEndTurn.Add(PoisonDamage);
    }

    public override void RemoveFunctionFromRules()
    {
        actor.actionsEndTurn.Remove(PoisonDamage);
    }

    IEnumerator PoisonDamage()
    {
        actor.LooseLife(nrStacked--);
        if (nrStacked == 0) Dismantle();
        yield return new WaitForSeconds(poisonAnimationTime);
    }

}
