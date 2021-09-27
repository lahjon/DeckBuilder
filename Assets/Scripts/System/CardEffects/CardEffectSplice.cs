using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardEffectSplice : CardEffect
{
    public override bool stackable { get { return false; } }
    public override bool isBuff { get { return true; } }

    public CardEffectSplice() : base()
    {
        OnEndTurn = _OnEndTurn;
    }

    protected override IEnumerator _OnEndTurn()
    {
        yield return CombatSystem.instance.StartCoroutine(RecieveInput(-nrStacked));
    }

}
