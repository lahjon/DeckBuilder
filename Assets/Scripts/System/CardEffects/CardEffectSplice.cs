using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectSplice : CardEffect
{
    public override bool isBuff { get { return true; } }

    public CardEffectSplice() : base()
    {
        OnEndTurn = _OnEndTurn;
    }

    public override IEnumerator _OnEndTurn()
    {
        yield return combatController.StartCoroutine(RecieveInput(-nrStacked));
    }

}
