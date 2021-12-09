using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectPoison : CardEffect
{

    private float poisonAnimationTime = 0.7f;

    public CardEffectPoison() : base()
    {
        OnNewTurn = null;
        OnEndTurn = null;
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
        actor.LooseLife(nrStacked);
        yield return new WaitForSeconds(poisonAnimationTime);
        yield return CombatSystem.instance.StartCoroutine(RecieveInput(-1));
    }

}
