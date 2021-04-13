using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuleEffectPoison : RuleEffect
{
    public override bool isBuff { get { return false; } }

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
        yield return null;
    }

    public override void OnNewTurn()
    {
    }

}
